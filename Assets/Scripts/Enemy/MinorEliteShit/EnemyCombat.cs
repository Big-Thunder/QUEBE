using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyBehaviour _enemyBehaviour;
    [SerializeField] private Transform enemyFirepoint;
    [SerializeField] private GameObject bulletObj;
    
    [Header("DevVariables")]
    [SerializeField] private float startTimeBetShots;
    [SerializeField] private float timeBetShots;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private bool canShoot;
    [SerializeField] private Collider[] collidersInShootingRange;
    [SerializeField] private float shootRange;
    [SerializeField] private LayerMask playerLayerMask;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyFirepoint = GameObject.Find("EnemyFirepoint").transform;
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
        timeBetShots = startTimeBetShots;
    }

    // Update is called once per frame
    void Update()
    {
        timeBetShots -= Time.deltaTime;
        if (canShoot)
        {
            Shoot();
        }

        collidersInShootingRange = Physics.OverlapSphere(transform.position, shootRange, playerLayerMask);

        if (collidersInShootingRange.Length > 0)
        {
            if (_enemyBehaviour.canFollowPlayer)
            {
                canShoot = true;
            }
            // StartCoroutine(_enemyBehaviour.Hide(collidersInShootingRange[0].transform));
        }
        else
        {
            canShoot = false;
        }
    }

    void Shoot()
    {
        if (timeBetShots <= 0f)
        {
            GameObject newBullet = Instantiate(bulletObj, enemyFirepoint.position, enemyFirepoint.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
            timeBetShots = startTimeBetShots; 
        }
        else 
        { 
            timeBetShots -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
