using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    public Transform playerTrans;
    [SerializeField] private Animator animator; 

    [Header("Dev Variables")]
    [SerializeField] private float updateSpeed;
    [SerializeField] private float startUpdateSpeed = 0.1f;

    [Header("Take Cover Vars")] 
    [SerializeField] private LayerMask hidableLayer;
    [SerializeField] private float hideRange;
    [Range(-1, 1)] [SerializeField] float hideSensitivity; //Lower is better
    [SerializeField] private Collider[] hideColliders = new Collider[10];


    public bool canFollowPlayer = false;
    public Vector3 coverPos = new Vector3();
    
    // Start is called before the first frame update
    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        // animator = GetComponent<Animator>();
     
        updateSpeed = startUpdateSpeed;
        
        if (canFollowPlayer)
        {
            StopCoroutine(Hide(playerTrans));
            StartCoroutine(FollowPlayer());
        }
        else
        {
            StopCoroutine(FollowPlayer());
            StartCoroutine(Hide(playerTrans));
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateSpeed -= Time.deltaTime;
        // FollowPlayerSimple();
        
        animator.SetFloat("vel_x", navMeshAgent.velocity.x);
        animator.SetFloat("vel_y", navMeshAgent.velocity.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            canFollowPlayer = !canFollowPlayer;
            if (canFollowPlayer)
            {
                StopCoroutine(Hide(playerTrans));
                StartCoroutine(FollowPlayer());
                // FollowPlayerSimple();
            }
            else
            {
                StopCoroutine(FollowPlayer());
                StartCoroutine(Hide(playerTrans));
            }
        }
    }

    IEnumerator FollowPlayer()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(updateSpeed);

        while (canFollowPlayer)
        {
            navMeshAgent.SetDestination(playerTrans.position);
            yield return waitForSeconds;
        }

        yield return null;
    }

    void FollowPlayerSimple()
    {
        if (updateSpeed <= 0f)
        {
            navMeshAgent.SetDestination(playerTrans.position);
            updateSpeed = startUpdateSpeed;
            Debug.Log("HEreaeufhnoau");
        }
        else
        {
            updateSpeed -= Time.deltaTime;
        }
    }

    public IEnumerator Hide(Transform target)
    {
        updateSpeed = startUpdateSpeed;
        
        for (int i = 0; i < hideColliders.Length; i++)
        {
            hideColliders[i] = null;
        }

        int hits = Physics.OverlapSphereNonAlloc(transform.position, hideRange, hideColliders, hidableLayer);
        Array.Sort(hideColliders, DistComparer);
        
        for (int i = 0; i < hits; i++)
        {
            if (NavMesh.SamplePosition(hideColliders[i].transform.position, out NavMeshHit hit, 50f, navMeshAgent.areaMask))
            {
                coverPos = hit.position;
                Debug.Log($"Hit = {hit.position}");
                if (!NavMesh.FindClosestEdge(hit.position, out hit, navMeshAgent.areaMask))
                {
                    Debug.Log("Problem");
                }

                if (Vector3.Dot(hit.normal, (target.position - hit.position).normalized) < hideSensitivity)
                {
                    navMeshAgent.SetDestination(hit.position); 
                    Debug.Log($"Going to {hit.position} - hit");
                    // coverPos = hit.position;
                    break;
                }
                else
                {
                    if (NavMesh.SamplePosition(hideColliders[i].transform.position -  (target.position - hit.position).normalized * 2, out NavMeshHit hit2, 50f, navMeshAgent.areaMask))
                    {
                        if (!NavMesh.FindClosestEdge(hit2.position, out hit2, navMeshAgent.areaMask))
                        {
                            Debug.Log("Problem");
                        }

                        if (Vector3.Dot(hit2.normal, (target.position - hit2.position).normalized) < hideSensitivity)
                        {
                            Debug.Log($"Going to {hit2.position} - hit2");
                            navMeshAgent.SetDestination(hit2.position);
                            // coverPos = hit2.position;
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"Cant Find Navmesh {hideColliders[i].name} {hideColliders[i].transform.position}");
            }
        }

        yield return null;
    }

    int DistComparer(Collider A, Collider B)
    {
        if (A == null && B == null)
        {
            return 0;
        }
        else if(A != null && B == null)
        {
            return -1;
        }else if (A == null && B != null)
        {
            return 1;
        }
        else
        {
            return Vector3.Distance(transform.position, A.transform.position).CompareTo(Vector3.Distance(transform.position, B.transform.position));
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(coverPos, 5f);
    }
}
