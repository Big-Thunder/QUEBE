using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject bodyToRotate;
    
    [Header("Dev Variables")]
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private float speed;
    [SerializeField] private float linearDrag;
    [SerializeField] private Vector3 lookPos;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        // animator = GetComponent<Animator>();
        cam = Camera.main;
        soundManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVector.Normalize();

        rb.drag = linearDrag;

        Vector3 normVel = rb.velocity.normalized;

        if (normVel.magnitude >= 0.25f)
        {
            animator.SetFloat("Vel_y", rb.velocity.x);
            animator.SetFloat("Vel_x", -rb.velocity.z);
            soundManager.StartPlayingOnLoop("QuebeWalk");
        }
        else
        {
            animator.SetFloat("Vel_y", 0f);
            animator.SetFloat("Vel_x", -0f);
            soundManager.StopPlayingOnLoop("QuebeWalk");
        }
        
        // RotToMovementDirection();
        // AimToMouse();
    }

    private void FixedUpdate()
    {
        // rb.velocity = new Vector3(moveVector.x, 0f, moveVector.y) * speed;
        Vector3 movement = new Vector3(moveVector.x, 0f, moveVector.y);
        rb.AddForce(movement * speed / Time.deltaTime);
        rb.angularVelocity = Vector3.zero;
    }

    void RotToMovementDirection()
    {
        bodyToRotate.transform.forward = moveVector;
    }

    void AimToMouse()
    {
        Plane plane = new Plane(Vector3.up, bodyToRotate.transform.position);
    
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    
        float dist;
    
        if (plane.Raycast(ray, out dist))
        {
            Vector3 point = ray.GetPoint(dist);
    
            Vector3 direction = point - bodyToRotate.transform.position;
    
            // Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
    
            bodyToRotate.transform.localRotation = Quaternion.Slerp(bodyToRotate.transform.localRotation, targetRotation, Time.deltaTime * 7f);
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7f);
            //transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
        }
    }
}
