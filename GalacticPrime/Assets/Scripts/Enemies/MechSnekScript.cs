using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechSnekScript : MonoBehaviour
{

    public float health;
    public Animator mechsnekanim;
    public Transform target;

    public float speed;
    public float stoppingDistance;
    public float followDistance;

    public AudioClip hurtSound;
    public AudioClip attackSound;
    public AudioClip snekSound;
    private AudioSource source;

    public PlayerHealth playerCurrentHealth;


    // Use this for initialization
    void Start()
    {
        mechsnekanim.SetBool("IsAlive", true);
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) < followDistance && health > 0)  // chasing condition
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, target.position) > stoppingDistance && Vector2.Distance(transform.position, target.position) < followDistance && health > 0)
        {
            StartCoroutine("Attacking");  // attacking condition
        }
    }

    IEnumerator Attacking()
    {
        mechsnekanim.SetBool("IsAttacking", true);
       // source.PlayOneShot(attackSound);
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);  // attacking
        yield return new WaitForSeconds(1);
        mechsnekanim.SetBool("IsAttacking", false);
    }

    public void Damage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            mechsnekanim.SetBool("IsAlive", false);  // dying when health 0
            Destroy(gameObject, (float).8);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Bullet"))  // getting hit by bullet
        {
            Destroy(other.gameObject);
            source.PlayOneShot(hurtSound);
            Damage(1);
        }
        if (other.gameObject.tag.Equals("Player"))  // damaging player
        {
            playerCurrentHealth.DamagePlayer(10);
        }
    }
}
