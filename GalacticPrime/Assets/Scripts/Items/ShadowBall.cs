using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBall : MonoBehaviour
{
    public Vector3 targetPos;
    private float speed = 900f;
    public WShadowBlade gun;
    private Rigidbody2D rb2d;
    public int damage;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {

        if (Vector3.Distance(transform.position, targetPos) > 0.3f) {
            //transform. = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            rb2d.velocity = (targetPos - transform.position).normalized * speed * Time.deltaTime;
        }
        else {
            gun.BallLanded(transform.position);
            DealDamage();
            Destroy(this.gameObject);
        }
        if (Vector3.Distance(transform.position, targetPos) < 0.5f) {
            anim.SetTrigger("Expand");
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Wall") {
            print("hit wall");
            Destroy(gameObject);
            gun.GoBack();
        }
    }

    void DealDamage() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in colliders) {
            if (collider.tag != "Player") {
                JHealthController healthCont = collider.GetComponent<JHealthController>();
                if (healthCont != null) {
                    healthCont.TakeDamage(damage);
                    Vector3 force = (transform.position - collider.transform.position).normalized * -1 * 20f;
                    force.y = 0;
                    JStatusController status = collider.GetComponent<JStatusController>();
                    if (status != null)
                        collider.GetComponent<JStatusController>().ApplyStun(0.5f);
                    collider.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
                }
            }
        }
    }
}
