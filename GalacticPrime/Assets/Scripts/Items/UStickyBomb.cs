using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UStickyBomb : MonoBehaviour, IItem {

    public bool playerItem = true;

    public GameObject bombExplosion;
    public GameObject explosionParticles;

    private Animator anim;
    private AudioSource audioSource;

    private float detonateTime = 3.5f;
    public float fuse;
    private bool fusing = false;
    private int dir;

    private float throwDuration = 0f;
    private bool released = false;
    private float releasedTime = 0f;

    private bool stuck = false;

    void Start() {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        throwDuration = 0.01f;
        released = false;

        if (!playerItem) {
            fuse = detonateTime;
            fusing = true;
            released = false;
            anim.SetTrigger("Fusing");
        }
    }

    void Update() {
        if (playerItem) {
            GameMaster.instance.inventory.UpdateCooldown(GetCooldown());
        }

        if (fusing) {
            fuse -= Time.deltaTime;
        }
        if (released) {
            releasedTime += Time.deltaTime;
        }

        dir = 0;
        if (GameMaster.instance.player.transform.position.x - transform.position.x > 0) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        if (fuse <= 0 && fusing) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 6.5f);
            foreach (Collider2D col in colliders) {
                JHealthController health = col.GetComponent<JHealthController>();
                if (health != null) {
                    float damage = (1 / Mathf.Pow(Vector3.Distance(transform.position, col.transform.position), 2)) * 110f;
                    health.TakeDamage((int)damage);
                }
            }
            audioSource.PlayOneShot(audioSource.clip);
            GameObject explosion = Instantiate(bombExplosion, new Vector3(transform.position.x, transform.position.y - 1f, 0f), Quaternion.identity);
            Destroy(explosion, 0.5f);
            GameObject particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            Destroy(particles, 1f);
            Destroy(this.gameObject);
        }

        if (playerItem) {
            if (fusing && Input.GetKey(KeyCode.J) && throwDuration < 2f) {
                released = false;
                throwDuration += Time.deltaTime;
            }

            if (Input.GetKeyUp(KeyCode.J) && fusing && !released && !stuck) {
                Release();
                GameMaster.instance.inventory.UseReusableItem();
                //int selectedSlot = GameMaster.instance.inventory.selectedSlot;
                //GameMaster.instance.inventory.Deselect();
                //GameMaster.instance.inventory.Select(selectedSlot);
                GetComponent<Collider2D>().isTrigger = false;
                Rigidbody2D rb2d = gameObject.AddComponent<Rigidbody2D>();
                rb2d.gravityScale = 1f;
                rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb2d.AddForce(new Vector2(dir * throwDuration * 10f, 0.1f * throwDuration * 3), ForceMode2D.Impulse);

            }
        }

    }

    public void Release() {
        transform.SetParent(null);
        stuck = false;
        released = true;
        releasedTime = 0f;
    }

    public void Use() {
        fuse = detonateTime;
        fusing = true;
        released = false;
        anim.SetTrigger("Fusing");
    }

    public float GetCooldown() {
        return 0f;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (playerItem && (releasedTime < 0.2f || !released)) {
            return;
        }
        if (!stuck) {
            stuck = true;
            transform.SetParent(collision.transform);
            Destroy(GetComponent<Rigidbody2D>());
            //GetComponent<Rigidbody2D>().mass = 0f;
            //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
}
