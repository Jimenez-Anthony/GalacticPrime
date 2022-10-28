using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfireLine : MonoBehaviour
{
    public int damage;
    public Vector3 startPos;

    public Rigidbody2D rb2d;
    private LineRenderer line;

    private float speed = 80f;
    public float yVel = 0f;
    public int faceDir = 0;
    private bool destroy = false;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameMaster.instance.player.GetComponent<Collider2D>());
        rb2d = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, startPos);
        line.SetPosition(1, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (destroy) {
            Destroy(gameObject);
            return;
        }

        line.SetPosition(0, startPos);
        line.SetPosition(1, transform.position);
        rb2d.velocity = new Vector3(faceDir * speed, yVel * 0.2f, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "Player") {
            return;
        }
        destroy = true;
        JHealthController healthCont = collision.gameObject.GetComponent<JHealthController>();
        if (healthCont != null) {
            healthCont.TakeDamage(damage);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(faceDir * 10f, 0.1f));
        }
    }
}
