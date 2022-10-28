using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechFlyScript : MonoBehaviour
{

    public float health;
    public Animator mechflyanim;
    public Transform target;

    public float speed;
    public float stoppingDistance;
    public float followDistance;

    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource source;

    public PlayerHealth playerCurrentHealth;


    // Use this for initialization
    void Start()
    {
        mechflyanim.SetBool("IsAlive", true);
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
       if(Vector2.Distance(transform.position, target.position) > stoppingDistance && Vector2.Distance(transform.position, target.position) < followDistance && health > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    public void Damage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            mechflyanim.SetBool("IsAlive", false);
            source.PlayOneShot(deathSound);
            Destroy(gameObject, (float).8);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            Destroy(other.gameObject);
            Damage(1);
            source.PlayOneShot(hitSound);
        }
        if (other.gameObject.tag.Equals("Player"))
        {
            playerCurrentHealth.DamagePlayer(10);
        }
    }
}
