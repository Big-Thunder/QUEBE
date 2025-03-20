using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDroneSpawner : MonoBehaviour
{
    public List<GameObject> drones;
    public List<EnemyDroneBehaviour> droneScripts;
    public Transform spawnDistancePoint; //Drones will measure their dist from this point and if they are far enough from this they will start their ai.
    public Vector3 spawnDistPointDir;

    // Start is called before the first frame update
    void Start()
    {
        drones.Clear();
        droneScripts.Clear();
        
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Enemy"))
            {
                children.Add(child);
            }else if (child.CompareTag("SpawnDistPoint"))
            {
                spawnDistancePoint = child;
                spawnDistPointDir = transform.position - spawnDistancePoint.position;
            }
        }

        foreach (Transform child in children)
        {
            EnemyDroneBehaviour enemyDroneBehaviour = child.GetComponent<EnemyDroneBehaviour>();
            drones.Add(child.gameObject); // Add children to the enemy list
            droneScripts.Add(enemyDroneBehaviour);
            enemyDroneBehaviour.spawnDistPoint = spawnDistancePoint;
            enemyDroneBehaviour.spawnDistPointDir = transform.position - spawnDistancePoint.position;
            child.parent = null; // Detach from the parent
            child.gameObject.SetActive(false); // deactivate the child
        }
    }

    // Update is called once per frame

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject enemy in drones)
            {
                if (!enemy.activeSelf) 
                {
                    enemy.SetActive(true);
                    Debug.Log($"EnemySpawner: Activated {enemy.name}");
                }
            }
        }
    }
}
