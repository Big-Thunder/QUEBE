using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKeyScript : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    
    public DoorKeyType doorKeyType;
    public PlayerStatsHandler playerStatsHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        playerStatsHandler = FindAnyObjectByType<PlayerStatsHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatsHandler.PickUpKey(this);
            meshRenderer.enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}
