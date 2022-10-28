using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateProjectile : MonoBehaviour
{
    private Transform player;
    public GameObject destructionParticles;
    private float throwDuration = 0.5f;
    public int damage = 10;
    private bool released = false;
    private bool launched = false;

    public DroppedItem droppedItem;
    public ItemStack drop;

    // Start is called before the first frame update
    void Start()
    {
        player = GameMaster.instance.player.transform;
        throwDuration = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        int dir = 0;
        if (player.transform.position.x - transform.position.x > 0) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        if (Input.GetKey(Keys.openCrate) && throwDuration < 2f) {
            throwDuration += Time.deltaTime;
        }

        if (Input.GetKeyUp(Keys.openCrate)) {
            if (!released) {
                released = true;
            }
            else {
                transform.SetParent(null);
                launched = true;
                GetComponent<Collider2D>().isTrigger = false;
                GameMaster.instance.player.GetComponent<JPlayerController>().holding = false;
                Rigidbody2D rb2d = gameObject.AddComponent<Rigidbody2D>();
                rb2d.gravityScale = 1f;
                rb2d.AddForce(new Vector2(dir * throwDuration * 10f, 0.1f * throwDuration * 3), ForceMode2D.Impulse);
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (!launched) {
            return;
        }
        if (collision.gameObject == player.gameObject) {
            return;
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D hit in hits) {
            if (hit.tag == "Player") {
                continue;
            }
            JHealthController health = hit.gameObject.GetComponent<JHealthController>();
            if (health != null) {
                health.TakeDamage(damage);
            }
        }
        GameMaster.instance.gameStats.cratesBorken++;
        GameObject particles = Instantiate(destructionParticles, transform.position, transform.localRotation) as GameObject;
        particles.transform.localScale = new Vector3(0.3f, 0.3f, 0f);
        DroppedItem item = Instantiate(droppedItem, transform.position, Quaternion.identity) as DroppedItem;
        item.itemStack = drop;
        //GetComponent<AudioSource>().Play();
        Destroy(particles, 0.5f);
        Destroy(this.gameObject);
    }
}
