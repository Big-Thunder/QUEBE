using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    private bool hasHit = false;
    public float dmg;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherName = other.transform.name;
        var playerName = "";
        if (other.transform.CompareTag("Player"))
        {
            if (playerName != otherName)
            {
                playerName = other.transform.name;
                Debug.Log("Hit Player: " + other.transform.name);
                PlayerStatsHandler playerStatsHandler = other.GetComponent<PlayerStatsHandler>();
                playerStatsHandler.TakeDmg(dmg);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("EnemyBullet: " + other.transform.name);
            Destroy(gameObject);
        }
    }
}
