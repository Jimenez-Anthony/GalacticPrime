using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechMageGuyScript : MonoBehaviour {

    public float health;
    public Animator mageguyanim;
    public Transform target;

    public float fireRange;
    public float fireRate;
    public GameObject mech3attack;

    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource source;

    private float nextFire;

    public PlayerHealth playerCurrentHealth;

    // Use this for initialization
	void Start () {
        mageguyanim.SetBool("IsAlive", true);
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if(Vector2.Distance(transform.position, target.position) < fireRange && Time.time > nextFire && health > 0)  // shoot conditions
        {
            mageguyanim.SetBool("IsAttacking", true);
            Fire();
        }
    }

    private void Fire()
    {
        nextFire = Time.time + fireRate;
        Instantiate(mech3attack, target.position, Quaternion.identity);  // shooting
    }

    public void Damage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            mageguyanim.SetBool("IsAlive", false);  // dying when health 0
            source.PlayOneShot(deathSound);
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Bullet"))  // getting hit by bullet
        {
            Destroy(other.gameObject);
            Damage(1);
            source.PlayOneShot(hitSound);
        }
        if (other.gameObject.tag.Equals("Player"))  // hitting player
        {
            playerCurrentHealth.DamagePlayer(10);
        }
    }

}
