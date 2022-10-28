using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UBomb : MonoBehaviour, IItem {

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


    void Start() {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        throwDuration = 0.01f;
    }

    void Update() {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (fusing) {
            fuse -= Time.deltaTime;
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

        if (fusing && Input.GetKey(KeyCode.J) && throwDuration < 2f) {
            throwDuration += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.J) && fusing && !released) {
            transform.SetParent(null);
            released = true;
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

    public void Use() {
        fuse = detonateTime;
        fusing = true;
        //GetComponent<AudioSource>().Play();
        released = false;
        anim.SetTrigger("Fusing");
    }

    public float GetCooldown() {
        return 0f;
    }



}
