using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourNew : MonoBehaviour
{
    
    
    public EnemyStats EnemyStats;
    public EnemyMovement EnemyMovement;
    public EnemyCombatNew EnemyCombatNew;
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public Transform Player;
    private Camera cam;

    public RaycastHit theHits;

    [Header("AI Vars")] 
    public float enemyRunAwayDist = 5f; //if less than this, hide
    public float shootDist = 20f; //if less than this and gtr than runAway, shoot;
    public float enemyFollowDist = 40f; //if gtr than this, follow
    [SerializeField] private float distanceToPlayer;
    [SerializeField] private String currentState;

    private void Start()
    {
        
        cam = Camera.main;
        EnemyMovement = GetComponent<EnemyMovement>();
        EnemyCombatNew = GetComponent<EnemyCombatNew>();
        EnemyStats = GetComponent<EnemyStats>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void HandleAnimationsNew()
    {
        animator.SetFloat("x",( Mathf.Clamp(navMeshAgent.velocity.magnitude, -2, 2)));
        animator.SetBool("isShooting", EnemyCombatNew.isShooting);
    }

    private void Update()
    {
        // Example condition: Trigger hiding when the player presses the "H" key
        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     EnemyMovement.TriggerHide(Player, true); // Start hiding
        // }
        // else if (Input.GetKeyDown(KeyCode.J))
        // {
        //     EnemyMovement.TriggerHide(Player, false); // Stop hiding
        // }

        // distanceToPlayer = Vector3.Distance(Player.position, transform.position);
        distanceToPlayer = Vector3.Distance(Player.position, transform.position);

        if (!EnemyStats.isDead) ////When NOT DEAD
        {
            HandleFollow();
            // HandleAnimations();
            HandleAnimationsNew();
        }
        else                    ////When DEAD
        {
            if (EnemyMovement.Agent.isActiveAndEnabled)
            {
                EnemyMovement.Agent.ResetPath();
            }
            
            if (EnemyMovement.MovementCoroutine != null)
            {
                StopCoroutine(EnemyMovement.MovementCoroutine);
                EnemyMovement.MovementCoroutine = null;
            }
            
            // EnemyMovement.Agent.velocity = Vector3.zero;
            EnemyMovement.Agent.enabled = false;
            animator.enabled = false;
            Debug.Log($"{transform.name}: Dead");
        }
    }

    void HandleFollow()
    {
        // distanceToPlayer = Vector3.Distance(transform.position, playerGroundPoint.position);
        distanceToPlayer = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(Player.position.x, 0f, Player.position.z));

        // if (distToPlayer <= enemyRunAwayDist)
        // {
        //     EnemyMovement.TriggerHide(Player, true); //Run Away
        //     EnemyCombatNew.isShooting = false;
        // }
        //
        // if (distToPlayer > enemyRunAwayDist && distToPlayer <= shootDist)
        // {
        //     FaceTarget(Player.position);
        //     EnemyCombatNew.Shoot(); //Shoot
        //     EnemyCombatNew.isShooting = true;
        // }
        //
        // if (distToPlayer > enemyFollowDist)
        // {
        //     FaceTarget(Player.position);
        //     EnemyCombatNew.isShooting = false;
        //     
        //     EnemyMovement.TriggerHide(Player, false);
        //     EnemyMovement.Agent.SetDestination(Player.position); // Follow
        // }
        
        
        if (EnemyMovement.Agent.destination == Player.position)
        {
            EnemyMovement.Agent.stoppingDistance = 2f;
        }
        else
        {
            EnemyMovement.Agent.stoppingDistance = 0f;
        }

        if (distanceToPlayer < enemyRunAwayDist && distanceToPlayer <= shootDist && distanceToPlayer <= enemyFollowDist)
        {
            // Enemy hides when the player is too close
            EnemyMovement.TriggerHide(Player, true);
            FaceTarget(Player.position); // Face the player while hiding
            EnemyCombatNew.isShooting = false; // Ensure no shooting while hiding
            currentState = "Hide";
        }
        else if (distanceToPlayer >= enemyRunAwayDist && distanceToPlayer <= shootDist && distanceToPlayer <= enemyFollowDist)
        {
            // Enemy hides and shoots
            EnemyMovement.TriggerHide(Player, true); // Enable hiding
            FaceTarget(Player.position); // Face the player
            EnemyCombatNew.Shoot(); // Execute shooting behavior
            
            currentState = "Hide and Shoot";
        }
        else if (distanceToPlayer > enemyRunAwayDist && distanceToPlayer > shootDist && distanceToPlayer <= enemyFollowDist)
        {
            // Enemy stops and shoots
            EnemyMovement.Agent.ResetPath(); // Stop moving
            FaceTarget(Player.position); // Face the player while shooting
            EnemyCombatNew.Shoot();
            EnemyMovement.TriggerHide(Player, false); // Ensure hiding is disabled
            currentState = "Shoot";
        }
        else if (distanceToPlayer > enemyRunAwayDist && distanceToPlayer > shootDist && distanceToPlayer > enemyFollowDist)
        {
            // Enemy follows the player
            EnemyMovement.TriggerHide(Player, false); // Ensure hiding is disabled
            EnemyMovement.Agent.SetDestination(Player.position);
            FaceTarget(Player.position); // Face the player while following
            EnemyCombatNew.isShooting = false; // Disable shooting
            currentState = "Follow";
        }
    }
    
    private void FaceTarget(Vector3 destination)
    {
        if (!EnemyStats.isDead)
        {
            Vector3 lookPos = destination - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.7f);  
        }
        else
        {
            //DeadCode
            return;
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (EnemyMovement.Player != null)
    //     {
    //         Gizmos.DrawLine(transform.position, Player.position);
    //     }
    // }
}
