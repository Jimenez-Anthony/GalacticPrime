using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManRocket : MonoBehaviour
{

    public float speed;
    public Vector3 targetLoc;
    public float radius;
    public int damage;

    public GameObject particles;


    void Update()
    {
        if (Vector3.Distance(transform.position, targetLoc) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, targetLoc, speed * Time.deltaTime);
        }
        else {
            GameObject clone = Instantiate(particles, transform.position, particles.transform.rotation) as GameObject;
            Destroy(clone, 2f);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D col in colliders) {
                if (col.GetComponent<RocketManBehavior>() != null) {
                    continue;
                }
                JHealthController health = col.GetComponent<JHealthController>();
                if (health != null) {
                    health.TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
                }
                JStatusController status = col.GetComponent<JStatusController>();
                if (status != null) {
                    status.ApplyStun(0.5f);
                }
                Rigidbody2D rb2d = col.GetComponent<Rigidbody2D>();
                if (rb2d != null) {
                    rb2d.AddForce(new Vector2((transform.position - col.transform.position).normalized.x * 25f, 10f));
                }
            }
            Destroy(gameObject);
        }
    }
}
