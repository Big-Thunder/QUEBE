using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float dieTime;
    [SerializeField] private float currentDieTime;
    public float dmg = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        currentDieTime = dieTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentDieTime -= Time.deltaTime;
        if (currentDieTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the parent exists
        if (other.transform.parent != null && other.transform.parent.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy");

            EnemyStats enemyStats = other.transform.parent.GetComponent<EnemyStats>();
            if (enemyStats != null) // Ensure EnemyStats component exists
            {
                Debug.Log(other.transform.parent.name);
                enemyStats.TakeDamage(dmg);
            }
            else
            {
                Debug.Log("EnemyStats component not found on the parent of the collider.");
            }
        }
        else
        {
            Debug.Log("Collider does not have a parent with tag 'Enemy' or parent is null.");
        }

        Destroy(gameObject);
    }
}
