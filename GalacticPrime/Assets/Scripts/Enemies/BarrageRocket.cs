using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageRocket : MonoBehaviour
{

    public int dir;
    public float lifeTime = 0.5f;
    private float timer;
    public float speed = 25f;
    private float heightVariation;
    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        heightVariation = Random.Range(-200f, 200f);
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime) {
            Destroy(gameObject);
        }
        rb2d.velocity = new Vector2(dir * speed * Time.deltaTime, heightVariation * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Enemy") {
            return;
        }
        if (collision.transform.tag == "Player") {
            collision.gameObject.GetComponent<JHealthController>().TakeDamage(1, JHealthController.DAMAGETYPE.Enemy);
        }
        Destroy(gameObject);
    }
}
