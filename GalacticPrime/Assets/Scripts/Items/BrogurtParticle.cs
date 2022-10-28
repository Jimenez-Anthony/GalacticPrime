using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrogurtParticle : MonoBehaviour
{

    public float life = 5f;
    private float timer;

    void Start()
    {
        life += Random.Range(-2f, 2f);
        timer = 0f;
    }

    void Update()
    {
        if (timer < life) {
            timer += Time.deltaTime;
        }
        else {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            return;
        }
        JHealthController health = collision.gameObject.GetComponent<JHealthController>();
        if (health != null) {
            if (Random.Range(0f, 1f) < 0.5f) {
                health.TakeDamage(1);
            }
        }
        JStatusController status = collision.gameObject.GetComponent<JStatusController>();
        if (status != null) {
            status.ApplySlow(0.5f, 0.05f);
        }
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
    }
}
