using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.forward = new Vector3(0f, moveVector.x, 0f);
        // transform.right = new Vector3(0f, 0f, moveVector.y);
    }
}
