using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapArrow : MonoBehaviour
{

    private float xScale;
    public int faceDir = 0;
    private float speed = 18f;
    public int damage = 0;
    public int poisonDamage = 1;
    public float poisonDuration = 5f;
    public GameObject poisonParticles;
    private Rigidbody2D rb2d;

    private float lifeTime = 0f;

    bool landed = false;
    float landedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        xScale = transform.localScale.x;
        rb2d = GetComponent<Rigidbody2D>();
        landed = false;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > 10f) {
            Destroy(gameObject);
        }

        transform.localScale = new Vector3(xScale * -faceDir, transform.localScale.y, transform.localScale.z);

        rb2d.velocity = new Vector3(speed * faceDir, 0f, 0f);

        if (landed && landedTime < 1f) {
            landedTime += Time.deltaTime;
        }
        else if (landed) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder")) {
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Decor")) {
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            Destroy(GetComponent<Collider2D>());
            rb2d.bodyType = RigidbodyType2D.Static;
            landed = true;
        }
        else {
            JHealthController health = collision.GetComponent<JHealthController>();
            if (health != null) {
                health.TakeDamage(damage, JHealthController.DAMAGETYPE.Environmental);
            }
            JStatusController status = collision.GetComponent<JStatusController>();
            if (status != null) {
                status.ApplyPoison(2, poisonDuration, poisonDamage, false);
                status.ApplyStun(0.2f);
                if (collision.gameObject.tag == "Player") {
                    GameObject particles = Instantiate(poisonParticles, GameMaster.instance.player.transform.GetChild(0), false) as GameObject;
                    Destroy(particles, poisonDuration);
                }
            }
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.AddForce(new Vector2(10f * faceDir, -10f), ForceMode2D.Impulse);
                //rb.velocity = new V
            }
            Destroy(gameObject);
        }

    }
}
