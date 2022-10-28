using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerBall : MonoBehaviour
{
    private float xScale;
    public int faceDir = 0;
    public float speed = 10f;
    private int damage = 3;
    private Rigidbody2D rb2d;

    private bool hitPlayer;
    private Collider2D target;
    private Transform targetOriginalParent;
    private float floatTimer;
    private bool hitWall;

    public GameObject particles;

    public StalkerBehavior stalker;

    void Start() {
        //xScale = transform.localScale.x;
        rb2d = GetComponent<Rigidbody2D>();
        hitPlayer = false;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), stalker.GetComponent<Collider2D>());
    }

    void Update() {
        //transform.localScale = new Vector3(xScale * faceDir, transform.localScale.y, transform.localScale.z);
        if (!hitPlayer) {
            rb2d.velocity = new Vector3(speed * faceDir, 0f, 0f);
        }
        else {
            if (floatTimer >= 0f) {
                floatTimer -= Time.deltaTime;
                if (!hitWall) {
                    rb2d.velocity = new Vector3(0f, 2f, 0f);
                }
                else {
                    rb2d.velocity = Vector3.zero;
                }
                target.transform.localPosition = Vector3.zero;
                target.transform.rotation = Quaternion.identity;
                FallDamage fall = target.GetComponent<FallDamage>();
                if (fall != null) {
                    print("ResetFallDamage");
                    fall.fallDuration = 0f;
                }
            }
            else {
                target.transform.SetParent(targetOriginalParent);
                //target.transform.localPosition = Vector3.zero;
                target.transform.rotation = Quaternion.identity;
                JHealthController healthCont = target.GetComponent<JHealthController>();
                healthCont.TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
                stalker.ballLanded = false;

                GameObject particlesClone = Instantiate(particles, transform.position, particles.transform.rotation) as GameObject;
                Destroy(particlesClone, 1f);
                Destroy(this.gameObject);
            }
        }

        if (hitWall) {
            rb2d.velocity = Vector3.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        //print(collision);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") && hitPlayer) {
            hitWall = true;
            if (!hitPlayer) {
                Destroy(this.gameObject);
            }
        }
        if (hitPlayer) {
            return;
        }
        if (collision.transform.gameObject == stalker.gameObject) {
            return;
        }

        if (collision.tag == "Player" || collision.tag == "Enemy") {
            target = collision;
            floatTimer = 3f;
            JStatusController status = collision.GetComponent<JStatusController>();
            if (status != null) {
                status.ApplyStun(3f);
            }
            hitPlayer = true;
            rb2d.velocity = Vector2.zero;
            stalker.ballLanded = true;
            stalker.ballLandingLocation = transform.position;

            transform.localScale = new Vector3(3f, 3f, 1f);
            targetOriginalParent = collision.transform.parent;
            collision.transform.SetParent(transform);
        }
        //JHealthController healthCont = collision.gameObject.GetComponent<JHealthController>();
        //if (healthCont != null) {
        //    healthCont.TakeDamage(damage);
        //}
        //Destroy(this.gameObject);
    }

}
