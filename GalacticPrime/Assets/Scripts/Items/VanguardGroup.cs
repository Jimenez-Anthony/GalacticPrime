using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanguardGroup : MonoBehaviour
{

    public Vector3 targetPos;
    public VanguardBlade vanguardBlade;
    private float speed = 900f;
    public bool facingRight = false;
    private Rigidbody2D rb2d;
    public int damage;
    //private Vector3[] bladeLocs;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        //bladeLocs = new Vector3[transform.GetChild(0).childCount];
        //for (int i = 0; i < bladeLocs.Length; i++) {
        //    bladeLocs[i] = transform.GetChild(0).GetChild(i).position;
        //}

    }

    void Update()
    {
        if (facingRight) {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Vector3.Distance(transform.position, targetPos) > 0.3f) {
            //transform. = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            rb2d.velocity = (targetPos - transform.position).normalized * speed * Time.deltaTime;
        }
        else {
            for (int i = 0; i < transform.GetChild(0).childCount; i++) {
                VanguardBlade bladeClone = Instantiate(vanguardBlade, transform.position, Quaternion.identity) as VanguardBlade;
                bladeClone.targetPos = transform.GetChild(0).GetChild(i).position;
                bladeClone.damage = damage;
            }
            Destroy(this.gameObject);

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
            healthCont.TakeDamage(damage);
        }
        JStatusController statusCont = collision.gameObject.GetComponent<JStatusController>();
        if (statusCont != null) {
            statusCont.ApplyStun(2f);
        }
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision);
        //Destroy(this.gameObject);
    }
}
