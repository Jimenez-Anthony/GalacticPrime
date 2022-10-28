using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanguardBlade : MonoBehaviour
{

    public Vector3 targetPos;
    private float speed = 900f;
    //public bool facingRight = false;
    private Rigidbody2D rb2d;
    public int damage;

    public float life = 5f;
    private float timer = 0f;
    private bool solid = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameMaster.instance.player.GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if (solid && timer > 0f) {
            timer -= Time.deltaTime;
        }
        if (solid && timer <= 0f) {
            Destroy(this.gameObject);
        }
        if (solid) {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameMaster.instance.player.GetComponent<Collider2D>());
        }

        if (Vector3.Distance(transform.position, targetPos) > 0.15f) {
            //transform. = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            rb2d.velocity = (targetPos - transform.position).normalized * speed * Time.deltaTime;
        }
        else {
            //Destroy(this.gameObject);
            if (!solid) {
                rb2d.bodyType = RigidbodyType2D.Static;
                BecomeSolid();
            }
        }
        if (Vector3.Distance(transform.position, targetPos) < 0.5f) {

        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag == "Player") {
            return;
        }
        JHealthController healthCont = collision.gameObject.GetComponent<JHealthController>();
        if (healthCont != null) {
            //healthCont.TakeDamage(damage);
        }
        JStatusController statusCont = collision.gameObject.GetComponent<JStatusController>();
        if (statusCont != null) {
            statusCont.ApplyStun(1f);
        }
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision);
        //Destroy(this.gameObject);
    }

    void BecomeSolid() {
        solid = true;
        timer = life;
        GetComponent<Collider2D>().isTrigger = false;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameMaster.instance.player.GetComponent<Collider2D>());

    }
}
