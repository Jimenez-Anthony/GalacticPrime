using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadismHealing : MonoBehaviour
{

    public float speed = 15f;
    public int healAmount = 1;

    private float randomMovementTimer = 0f;
    private Vector2 movement;

    private Rigidbody2D rb2d;

    public GameObject healParticles;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, GameMaster.instance.player.transform.position) > 0.3f) {
            transform.position = Vector3.MoveTowards(transform.position, GameMaster.instance.player.transform.position, speed * Time.deltaTime);
        }
        else {
            JHealthController health = GameMaster.instance.player.GetComponent<JHealthController>();
            health.Heal(healAmount, true);
            GameObject particles = Instantiate(healParticles, GameMaster.instance.player.transform.GetChild(0), false);
            particles.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Destroy(particles, 0.5f);
            Destroy(gameObject);
        }

        if (randomMovementTimer >= 0f) {
            randomMovementTimer -= Time.deltaTime;
        }
        else {
            randomMovementTimer = 0.5f;
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
    }

    void FixedUpdate() {
        rb2d.AddForce(movement * (speed / 1.5f));
    }
}
