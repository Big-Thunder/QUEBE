using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NavMeshExp : MonoBehaviour
{

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float collidersRange;
    [SerializeField] private Collider[] colliders = new Collider[10];
    [SerializeField] private LayerMask collidersLayer;

    [SerializeField] private float updateTime;
    
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // int hits = Physics.OverlapSphereNonAlloc(transform.position, collidersRange, colliders, collidersLayer);
        StartCoroutine(GotoNearest());
    }

    // Update is called once per frame
    void Update()
    {
        // int hits = Physics.OverlapSphereNonAlloc(transform.position, collidersRange, colliders, collidersLayer);
        // Debug.Log($"hits: {hits}");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GotoNearest());
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(GotoNearest());
        }
    }

    IEnumerator GotoNearest()
    {
        WaitForSeconds wait = new WaitForSeconds(updateTime);
        
        int hits = Physics.OverlapSphereNonAlloc(transform.position, collidersRange, colliders, collidersLayer);
        
        Array.Sort(colliders, (a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
        
        // Debug.Log($"hits: {hits}");

        for (int i = 0; i<hits; i++)
        {
            if (NavMesh.SamplePosition(colliders[0].transform.position, out NavMeshHit hit, 5f, navMeshAgent.areaMask))
            {
                navMeshAgent.SetDestination(hit.position);
                Debug.Log(colliders[0].transform.position + ": " + colliders[0]);
                break;
            }
        }

        yield return wait;
    }
}
