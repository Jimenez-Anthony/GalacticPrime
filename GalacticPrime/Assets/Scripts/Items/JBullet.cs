using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBullet : MonoBehaviour
{
    private float xScale;
    public int faceDir = 0;
    public float speed = 10f;
    public int damage = 0;
    private Rigidbody2D rb2d;

    void Start()
    {
        xScale = transform.localScale.x;
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.localScale = new Vector3(xScale * faceDir, transform.localScale.y, transform.localScale.z);
        rb2d.velocity = new Vector3(speed * faceDir, 0f, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "Player") {
            return;
        }
        //print(collision);
        JHealthController healthCont = collision.gameObject.GetComponent<JHealthController>();
        if (healthCont != null) {
            healthCont.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }
}
