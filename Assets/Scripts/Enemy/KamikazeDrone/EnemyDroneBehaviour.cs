using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyDroneBehaviour : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Transform playerTrans;
    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletObj;
    [SerializeField] private Transform firePoint_R;
    [SerializeField] private Transform firePoint_L;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [FormerlySerializedAs("droneEnemyStats")] 
    [SerializeField] private EnemyStats enemyStats;

    [SerializeField] private Rigidbody rb;

    [Header("Dev Vars")] 
    [SerializeField] private Collider[] colliders = new Collider[1];
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float updateSpeed = 0.1f;
    public bool hasSpawned = false;
    public Transform spawnDistPoint;
    [SerializeField] private float distFromSpawnPoint;
    public Vector3 spawnDistPointDir;
    [SerializeField] private float spawnSpeed = 40f;

    [Header("Combat Vars")] 
    [SerializeField] private float startTimeBetShots_R;
    [SerializeField] private float timeBetShots_R;
    [SerializeField] private float startTimeBetShots_L;
    [SerializeField] private float timeBetShots_L;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootRange;
    [SerializeField] private float lowHealthSpeed;
    [SerializeField] private float stoppingDistBeforeLowHp;

    private Coroutine moveCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyStats = GetComponent<EnemyStats>();
        meshRenderer = GameObject.Find($"{transform.name}/DroneGraphics").GetComponent<MeshRenderer>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        spawnDistPointDir = spawnDistPoint.position - transform.position;

        timeBetShots_R = startTimeBetShots_R;
        hasSpawned = false;

        StopMoveCoroutine();
    }

    void Update()
    {
        if (!hasSpawned)
        {
            SpawnThroughPortal();
        }
        else
        {
            FollowAndAttackPlayer();
        }
    }

    void SpawnThroughPortal()
    {
        distFromSpawnPoint = Vector3.Distance(transform.position, spawnDistPoint.position);

        if (distFromSpawnPoint >= 10f)
        {
            EnableNavMeshAgent();
            hasSpawned = true; // Mark the spawn as complete
        }
        else
        {
            DisableNavMeshAgent();
            MoveBackToSpawn();
        }
    }

    void FollowAndAttackPlayer()
    {
        if (enemyStats.hp > 0f)
        {
            HandleHealth();

            // Continue following the player and attacking
            if (moveCoroutine == null)
            {
                StartMoveCoroutine();
            }
        }
        else
        {
            Die();
        }
    }

    void HandleHealth()
    {
        if (enemyStats.hp > 0f)
        {
            timeBetShots_R -= Time.deltaTime;

            int hits = Physics.OverlapSphereNonAlloc(transform.position, shootRange, colliders, playerLayer);

            if (hits > 0 && enemyStats.hp > 30f)
            {
                Shoot();
                navMeshAgent.stoppingDistance = stoppingDistBeforeLowHp;
            }

            if (enemyStats.hp < 30f)
            {
                navMeshAgent.speed = lowHealthSpeed;
                navMeshAgent.stoppingDistance = 0f;
                if (Vector3.Distance(transform.position, playerTrans.position) < 5f)
                {
                    if (moveCoroutine != null)
                    {
                        PlayerStatsHandler playerStatsHandler = playerTrans.GetComponent<PlayerStatsHandler>();
                        playerStatsHandler.TakeDmg(playerStatsHandler.maxHp * 0.5f);
                        Die();
                    }
                }
            }
        }
        else
        {
            Die();
        }
    }

    void EnableNavMeshAgent()
    {
        if (!navMeshAgent.enabled)
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                }
                else
                {
                    Debug.LogError("Enemy is not on a valid NavMesh.");
                }
            }
            navMeshAgent.enabled = true;
        }
    }

    void DisableNavMeshAgent()
    {
        if (!hasSpawned) // Only disable the NavMeshAgent during the spawn phase
        {
            StopMoveCoroutine();
            navMeshAgent.enabled = false;
            timeBetShots_L = 10f;
            timeBetShots_R = 10f;
        }
    }

    void MoveBackToSpawn()
    {
        if (isActiveAndEnabled && spawnDistPoint != null)
        {
            LookAtPoint(playerTrans.position);
            Vector3 dir = (spawnDistPoint.position - transform.position).normalized;
            transform.position += -spawnDistPointDir * spawnSpeed * Time.deltaTime;
        }
    }

    void StartMoveCoroutine()
    {
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(FollowPlayer());
        }
    }

    void StopMoveCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    void Die()
    {
        timeBetShots_R = 10f;
        navMeshAgent.ResetPath();
        navMeshAgent.speed = 0f;
        meshRenderer.enabled = false;
        StopMoveCoroutine();
        Destroy(gameObject, 1f);
    }

    void LookAtPoint(Vector3 p)
    {
        Vector3 point = p;
        Vector3 direction = point - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 7f);
    }

    IEnumerator FollowPlayer()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(updateSpeed);

        while (Vector3.Distance(transform.position, playerTrans.position) > 1f)
        {
            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(playerTrans.position);
            }
            else
            {
                Debug.LogWarning("NavMeshAgent is not active or not on a NavMesh.");
            }
            yield return waitForSeconds;
        }
        yield return null;
    }

    void Shoot()
    {
        if (timeBetShots_R <= 0f)
        {
            GameObject newBullet_R = Instantiate(bulletObj, firePoint_R.position, firePoint_R.rotation);
            newBullet_R.GetComponent<Rigidbody>().velocity = newBullet_R.transform.forward * bulletSpeed;
            timeBetShots_R = startTimeBetShots_R;
        }
        else
        {
            timeBetShots_R -= Time.deltaTime;
        }

        if (timeBetShots_L <= 0f)
        {
            GameObject newBullet_L = Instantiate(bulletObj, firePoint_L.position, firePoint_L.rotation);
            newBullet_L.GetComponent<Rigidbody>().velocity = newBullet_L.transform.forward * bulletSpeed;
            timeBetShots_L = startTimeBetShots_L;
        }
        else
        {
            timeBetShots_L -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
