using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsHandler : MonoBehaviour
{
    public float maxHp;
    public float hp;

    public Slider healthBar;

    public List<DoorKeyScript> doorKeyScripts;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find($"{transform.parent.parent.name}/Canvas/HealthBar").GetComponent<Slider>();
        healthBar.maxValue = maxHp;
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = hp;
    }

    public void TakeDmg(float dmg)
    {
        hp -= dmg;
    }

    public void PickUpKey(DoorKeyScript _doorKeyScript)
    {
        doorKeyScripts.Add(_doorKeyScript);
        Debug.Log("Pickup Key");
    }
}
