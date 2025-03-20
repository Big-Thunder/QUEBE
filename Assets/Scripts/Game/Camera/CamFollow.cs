using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    [SerializeField] private Vector3 posOffset;
    [SerializeField] private Vector3 RotOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player_Model").transform;
        posOffset = transform.position - playerTrans.position;
        RotOffset = transform.rotation.eulerAngles;
        Debug.Log("Player: " + playerTrans.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = new Vector3(playerTrans.position.x + posOffset.x, posOffset.y,
            playerTrans.position.z + posOffset.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 7f);
        // transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 7f);
        // transform.position = Vector3.Lerp(transform.position, posOffset, Time.deltaTime * 7f);
        transform.rotation = Quaternion.Euler(RotOffset);
    }
}
