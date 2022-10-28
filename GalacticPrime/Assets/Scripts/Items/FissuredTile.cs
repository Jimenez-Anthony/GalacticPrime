using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FissuredTile : MonoBehaviour
{

    public Sprite sprite;
    private SpriteRenderer sr;
    private Rigidbody2D rb2d;
    private int damage = 3;

    public ParticleSystem particles;

    private List<Collider2D> hitColliders;

    void Start()
    {
        hitColliders = new List<Collider2D>();

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(new Vector2(0f, 12f), ForceMode2D.Impulse);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            return;
        }
        if (!hitColliders.Contains(collision)) {
            hitColliders.Add(collision);
            JHealthController health = collision.GetComponent<JHealthController>();
            if (health != null) {
                StartCoroutine(FissureDamage(collision.gameObject, health));
            }
        }

    }

    IEnumerator FissureDamage(GameObject obj, JHealthController health) {

        Rigidbody2D colliderRb = obj.GetComponent<Rigidbody2D>();

        if (colliderRb != null) {

            ParticleSystem particleClone = Instantiate(particles, new Vector2(transform.position.x, transform.position.y - 1f), particles.transform.rotation);
            particleClone.textureSheetAnimation.SetSprite(0, sprite);
            Destroy(particleClone.gameObject, 0.5f);

            //float gravityScale = colliderRb.gravityScale;
            //colliderRb.gravityScale = 85f;
            if (colliderRb.gravityScale < 80f) {
                JStatusController status = obj.GetComponent<JStatusController>();
                if (status != null) {
                    status.ApplyStun(5f);
                }

                colliderRb.AddForce(new Vector2(0f, 80f), ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.25f);

            //colliderRb.gravityScale = gravityScale;


            health.TakeDamage(damage);
        }

    }
}
