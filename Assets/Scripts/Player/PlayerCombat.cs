using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Transform firePointGrenade;
    [SerializeField] public GameObject bulletObj;
    [SerializeField] public GameObject grenadeObj;
    [SerializeField] public SoundManager soundManager;
    // [SerializeField] private CamShake camShake;
    
    [Header("Game Variables")] 
    [SerializeField] public float fireRate = 0f;
    [SerializeField] float timeBetShots_R = 0f;
    [SerializeField] float timeBetShots_L = 0f;
    [SerializeField] float timeBetShots_Grenade = 0f;
    [SerializeField] float startTimeBetShots_R = 0f;
    [SerializeField] float startTimeBetShots_L = 0f;
    [SerializeField] float startTimeBetShots_Grenade = 0f;
    [SerializeField] public float bulletSpeed_L = 0f;
    [SerializeField] public float bulletSpeed_R = 0f;
    [SerializeField] public float Dmg_R = 0f;
    [SerializeField] public float Dmg_L = 0f;

    // Start is called before the first frame update
    void Start()
    {
        firePointLeft = GameObject.Find("Firepoint_L").transform;
        firePointRight = GameObject.Find("Firepoint_R").transform;
        firePointGrenade = GameObject.Find("Firepoint_Grenade").transform;
        soundManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SoundManager>();
        // camShake = Camera.main.GetComponent<CamShake>();
        
        timeBetShots_L = startTimeBetShots_L;
        timeBetShots_R = startTimeBetShots_R;
        timeBetShots_Grenade = startTimeBetShots_Grenade;
    }

    // Update is called once per frame
    void Update()
    {
        timeBetShots_L -= Time.deltaTime;
        timeBetShots_R -= Time.deltaTime;
        timeBetShots_Grenade -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot(1);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Shoot(2);
        }

        if (Input.GetKey(KeyCode.G))
        {
            ShootGrenade();
        }
    }

    void Shoot(int firePointIndex) //Firepoint index -> left = 1, right = 2
    {
        if (firePointIndex == 1)
        {
            if (timeBetShots_L <= 0f)
            {
                GameObject newBullet = Instantiate(bulletObj, firePointLeft.position, firePointLeft.rotation);
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed_L;
                newBullet.GetComponent<BulletBehaviour>().dmg = Dmg_L;
                timeBetShots_L = startTimeBetShots_L;
                // camShake.ShakeSmall();
                soundManager.Play("PlayerBullet");
                
            }
            else
            {
                timeBetShots_L -= Time.deltaTime;
            }
        }

        if (firePointIndex == 2)
        {
            if (timeBetShots_R <= 0f)
            {
                GameObject newBullet = Instantiate(bulletObj, firePointRight.position, firePointRight.rotation);
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed_R;
                newBullet.GetComponent<BulletBehaviour>().dmg = Dmg_R;
                timeBetShots_R = startTimeBetShots_R;
                // camShake.ShakeSmall();
                // SoundManager.PlaySoundOnce(SoundType.PlayerBullet, 0.3f);
            }
            else
            {
                timeBetShots_R -= Time.deltaTime;
            }
        }
    }

    void ShootGrenade()
    {
        if (timeBetShots_Grenade <= 0f)
        {
            GameObject newGrenade = Instantiate(grenadeObj, firePointGrenade.position, firePointGrenade.rotation);
            newGrenade.GetComponent<Rigidbody>().AddForce(firePointGrenade.forward * 15f, ForceMode.Impulse);
            timeBetShots_Grenade = startTimeBetShots_Grenade;
        }
    }
}
