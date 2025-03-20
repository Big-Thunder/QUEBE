using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public class BossBehaviour : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private GameObject bulletObj, bulletObj_bomb, bulletObj_rain;
    [SerializeField] private Transform firePoint, firePoint_bomb, firePoint_rain;
    [SerializeField] private GameObject bossRainAttackWarningImage;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private MultiAimConstraint multiAimConstraint;
    
    [Header("Game Variables")]
    [SerializeField] private float startTimeBetShots, gunFireDuration;
    [SerializeField] private int countsAfterBombsToRain, countsOfBombRain;
    [SerializeField] private float maxHp, gunDmg, bombDmg, rainDmg;
    [SerializeField] private float aimSpeed, bulletSpeed;
    
    private float timeBetShots, hp;
    private int currentCountsAfterBombsToRain, currentCountsOfBombRain;
    
    [Header("Dev Variables")]
    private bool canShoot;
    private Coroutine shootRainCoroutine, shootStatesCoroutine;
    [SerializeField] private float bossRotationOffset;
    [SerializeField] private float gunOffset;
    [SerializeField] private float bombOffset;

    void Start()
    {
        animator = transform.Find("BossGFX").GetComponent<Animator>();
        enemyStats = transform.Find("BossGFX").GetComponent<EnemyStats>();
        multiAimConstraint = transform.Find("BossGFX/Rig 1").GetComponent<MultiAimConstraint>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        
        timeBetShots = startTimeBetShots;
        hp = maxHp;
        currentCountsAfterBombsToRain = 0;
        currentCountsOfBombRain = 0;
        
        shootStatesCoroutine = StartCoroutine(ShootStates());
    }

    void Update()
    {
        if (enemyStats.isDead)
        {
            Debug.Log("Boss Dead - Stopping Coroutines");
            StopAllCoroutines();
            return;
        }

        timeBetShots -= Time.deltaTime;

        if (canShoot && timeBetShots <= 0f)
        {
            Shoot();
        }

        HandleAnimations();
        var data = multiAimConstraint.data;
        
        // Correctly compute the offset taking into account boss rotation
        data.offset = new Vector3(0f, FaceTarget(firePoint.position, playerTrans.position, transform).y + bossRotationOffset, 0f);  
        
        multiAimConstraint.data = data; // Apply changes to constraint
        //Fix the boss aiming
    }

    void HandleAnimations()
    {
        animator.SetBool("isShooting", canShoot);
    }

    IEnumerator ShootStates()
    {
        Debug.Log("ShootStates started");
        
        while (!enemyStats.isDead)
        {
            Debug.Log($"ShootStates Loop - BombsToRain Count: {currentCountsAfterBombsToRain}");

            currentCountsAfterBombsToRain = (currentCountsAfterBombsToRain + 1) % countsAfterBombsToRain;

            canShoot = true;
            bossRotationOffset = gunOffset;
            yield return new WaitForSeconds(gunFireDuration);
            canShoot = false;
            bossRotationOffset = bombOffset;
            yield return new WaitForSeconds(1f);

            Debug.Log("Before Shoot Bomb");
            Shoot_Bomb();
            canShoot = true;

            if (currentCountsAfterBombsToRain == countsAfterBombsToRain - 1)
            {
                if (shootRainCoroutine == null)
                {
                    shootRainCoroutine = StartCoroutine(ShootRain());
                }
            }
            else if (shootRainCoroutine != null && currentCountsOfBombRain >= countsOfBombRain)
            {
                StopCoroutine(shootRainCoroutine);
                shootRainCoroutine = null;
                currentCountsOfBombRain = 0;
            }
        }
    }

    void Shoot()
    {
        if (currentCountsOfBombRain == 0)
        {
            GameObject newBullet = Instantiate(bulletObj, firePoint.position, firePoint.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * newBullet.transform.forward;
            newBullet.GetComponent<EnemyBulletBehaviour>().dmg = gunDmg;
            timeBetShots = startTimeBetShots;
        }
    }
    
    void Shoot_Bomb()
    {
        Debug.Log("ShootBomb executed");
        
        if (currentCountsOfBombRain == 0)
        {
            GameObject newBullet = Instantiate(bulletObj_bomb, firePoint_bomb.position, firePoint_bomb.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * newBullet.transform.forward;
            newBullet.GetComponent<EnemyBulletBehaviour>().dmg = bombDmg;
            animator.SetTrigger("Bomb");
        }
    }

    IEnumerator ShootRain()
    {
        while (currentCountsOfBombRain < countsOfBombRain)  // Run only if the count is less than the limit
        {
            GameObject newBullet = Instantiate(bulletObj_rain, firePoint_rain.position, firePoint_rain.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * newBullet.transform.forward;

            yield return new WaitForSeconds(1f);

            Vector3 randomPoint = GetRandomPointAroundPoint(playerTrans.position, 5f);

            // Quaternion targetRotation = Quaternion.LookRotation(randomPoint, transform.position);
            // newBullet.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
            float timeToReach = Vector3.Distance(randomPoint, newBullet.transform.position) / bulletSpeed;
            StartCoroutine(ActivateRainAttackWarning(randomPoint, timeToReach));

            newBullet.transform.forward = (randomPoint - newBullet.transform.position).normalized;
            newBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * (randomPoint - newBullet.transform.position).normalized;
            newBullet.GetComponent<EnemyBulletBehaviour>().dmg = rainDmg;

            currentCountsOfBombRain++;  // Increment the count
            canShoot = false;
            animator.SetBool("isRaining_ShootSide", true);

            if (currentCountsOfBombRain >= countsOfBombRain)  // Check if the max count is reached
            {
                StopCoroutine(shootRainCoroutine);  // Stop the coroutine when the max count is reached
                shootRainCoroutine = null;  // Reset the coroutine reference
            }
        }
        currentCountsOfBombRain = 0;
        animator.SetBool("isRaining_ShootSide", false);
        canShoot = true;
    }
    
    Vector3 FaceTarget(Vector3 gunPos, Vector3 playerPos, Transform bossTransform)
    {
        Vector3 directionToPlayer = playerPos - gunPos;
        directionToPlayer.y = 0; // Keep it horizontal

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            // Get world rotation to face the player
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Convert to local space relative to the boss
            Quaternion localRotation = Quaternion.Inverse(bossTransform.rotation) * targetRotation;

            // Return local Euler angles as offset
            return localRotation.eulerAngles;
        }

        return Vector3.zero; // No rotation needed
    }



    Vector3 GetRandomPointAroundPoint(Vector3 point, float range)
    {
        Vector3 randomXZ = new Vector3(
            point.x + Random.Range(-range, range),
            point.y + 10f,  // Start raycast from above
            point.z + Random.Range(-range, range)
        );

        // Raycast down to find the ground only
        if (Physics.Raycast(randomXZ, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            randomXZ.y = hit.point.y;  // Set Y to the ground level
        }
        else
        {
            Debug.LogWarning("Raycast failed to find ground! Defaulting to y = 0.");
            randomXZ.y = 0f;  // Default to 0 if no ground is found
        }

        Debug.Log("Final Random Point: " + randomXZ);
        return randomXZ;
    }

    IEnumerator ActivateRainAttackWarning(Vector3 attackPosition, float timeToReach)
    {
        GameObject warning = Instantiate(bossRainAttackWarningImage, attackPosition, Quaternion.Euler(0f, 0f, 0f));
        warning.transform.position = new Vector3(attackPosition.x, attackPosition.y, attackPosition.z);
        // warning.transform.up = Vector3.up;
        warning.SetActive(true);

        yield return new WaitForSeconds(timeToReach);

        warning.SetActive(false);
    }
}
