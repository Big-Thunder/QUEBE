using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public enum EnemyType
    {
        Drone,
        Minor,
        EliteMinor,
        Elite,
        Boss
    }

    public EnemyType enemyType;
    public GameObject damageTaker;
    
    public float hp;
    public float maxHp;
    public bool canBomb;
    public bool isDead;

    // public Slider hpBar;
    
    // Start is called before the first frame update
    void Start()
    {
        damageTaker = GameObject.Find($"{transform.name}/DamgeTaker");
        
        hp = maxHp;
        if (this.enemyType == EnemyType.Elite)
        {
            canBomb = true;
        }
        else
        {
            canBomb = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead && hp <= 0f) //won't check if already isDead
        {
            isDead = true;
            damageTaker.SetActive(false);
        }
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
    }
}
