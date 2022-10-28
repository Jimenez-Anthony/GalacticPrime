using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WNail : MonoBehaviour, IItem {

    public float attackCooldown = 0.5f;
    private float attackTimer;
    public int energyCost = 5;
    public int damage = 5;
    public float stunDuration = 0.5f;

    private JPlayerController player;
    private AudioSource audioSource;
    private int dir = 0;

    public GameObject leftParticles;
    public GameObject rightParticles;
    private Animator anim;

    void Start()
    {
        player = GameMaster.instance.player.GetComponent<JPlayerController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        attackTimer = 0f;
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
    }

    void Update()
    {
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }

        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (player.facingRight) {
            dir = 1;
        }
        else {
            dir = -1;
        }
    }

    public float GetCooldown() {
        return attackTimer / attackCooldown;
    }

    public void Use() {
        if (attackTimer <= 0f && GameMaster.instance.playerEnergy.UseEnergy(energyCost)) {
            attackTimer = attackCooldown;
            anim.SetTrigger("Attack");
            player.GetComponent<Animator>().SetTrigger("Slash");
            audioSource.Play();

            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.25f, new Vector2(dir, 0f), 1f);
            foreach (RaycastHit2D hit in hits) {
                if (hit.transform == gameObject) {
                    continue;
                }
                if (hit.transform.tag == "Player") {
                    continue;
                }

                JHealthController health = hit.transform.GetComponent<JHealthController>();
                if (health != null) {
                    health.TakeDamage(damage);
                    if (dir == 1) {
                        GameObject particles = Instantiate(rightParticles, new Vector3(transform.position.x + 0.5f, transform.position.y, 0f), Quaternion.identity) as GameObject;
                        Destroy(particles, 1f);
                    }
                    else {
                        GameObject particles = Instantiate(leftParticles, new Vector3(transform.position.x - 0.5f, transform.position.y, 0f), Quaternion.identity) as GameObject;
                        Destroy(particles, 1f);
                    }
                }
                JStatusController status = hit.transform.GetComponent<JStatusController>();
                if (status != null) {
                    status.ApplyStun(stunDuration);
                }
                Rigidbody2D rb2d = hit.transform.GetComponent<Rigidbody2D>();
                if (rb2d != null) {
                    rb2d.AddForce(new Vector2(10f, 5f), ForceMode2D.Impulse);
                }

            }

            GameMaster.instance.inventory.UseDurability(1);
        }
    }
}
