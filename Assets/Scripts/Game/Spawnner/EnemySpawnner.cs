using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyObjects = new List<GameObject>(); 

    // Start is called before the first frame update
    void Start()
    {
        enemyObjects.Clear();
        
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            enemyObjects.Add(child.gameObject); // Add children to the enemy list
            child.parent = null; // Detach from the parent
            child.gameObject.SetActive(false); // deactivate the child
        }

        Debug.Log($"{enemyObjects.Count} enemies initialized in {gameObject.name}");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject enemy in enemyObjects)
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