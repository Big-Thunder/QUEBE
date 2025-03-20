using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatNew : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private GameObject bulletObj;
    [SerializeField] public Transform firePoint;
    [SerializeField] private float timeBetShots;
    [SerializeField] private float startTimeBetShots;
    [SerializeField] private float bulletSpeed;
    public bool isShooting;

    public GameManager gameManager;
    [SerializeField] private GameObject bombObject;
    [SerializeField] private float bombSpeed;

    public float dmg;
    
    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SoundManager>();
        enemyStats = GetComponent<EnemyStats>();
        
        timeBetShots = startTimeBetShots;

        gameManager = Camera.main.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyStats.isDead)
        {
            timeBetShots -= Time.deltaTime;
            gameManager.timeBetBombs -= Time.deltaTime;
        }
        else
        {
            timeBetShots = 100f;
            gameManager.timeBetBombs = 100f;
        }
    }
    
    public void Shoot()
    {
        isShooting = true;
        if (timeBetShots <= 0f)
        {
            GameObject newBullet = Instantiate(bulletObj, firePoint.position, firePoint.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
            newBullet.GetComponent<EnemyBulletBehaviour>().dmg = dmg;
            soundManager.Play("EnemyBullet");
            timeBetShots = startTimeBetShots; 
        }
        else
        {
            timeBetShots -= Time.deltaTime;
        }
    }

    public void Bomb()
    {
        if (gameManager.timeBetBombs <= 0f)
        {
            GameObject newBomb = Instantiate(bombObject, firePoint.position, firePoint.rotation);
            newBomb.GetComponent<Rigidbody>().velocity = newBomb.transform.forward * bombSpeed;
            gameManager.timeBetBombs = gameManager.startTimeBetBombs;
        }
        else
        {
            gameManager.timeBetBombs -= Time.deltaTime;
        }
    }
}
