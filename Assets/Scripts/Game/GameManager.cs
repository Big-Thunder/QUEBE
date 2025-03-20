using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mouseFollowerObj;
    [SerializeField] private GameObject playerObj;
    public float startTimeBetBombs;
    public float timeBetBombs;

    [SerializeField] private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Physics.IgnoreLayerCollision(6, 7);
        Physics.IgnoreLayerCollision(8, 11);

        timeBetBombs = startTimeBetBombs;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseFollower();
        timeBetBombs -= Time.deltaTime;
    }
    
    void HandleMouseFollower()
    {
        Plane plane = new Plane(Vector3.up, playerObj.transform.position);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        float dist;

        if (plane.Raycast(ray, out dist))
        {
            Vector3 point = ray.GetPoint(dist);

            mouseFollowerObj.transform.position = point;
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7f);
            //transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
        }
    }
}
