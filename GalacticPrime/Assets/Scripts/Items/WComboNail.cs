using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WComboNail : MonoBehaviour, IItem {

    public float attackCooldown = 0.5f;
    private float attackTimer;
    public int energyCost = 5;
    public int damage = 5;
    public float stunDuration = 0.5f;

    private JPlayerController player;
    private AudioSource audioSource;
    private int dir = 0;

    private bool leftHit;
    private bool rightHit;
    private float comboResetTimer;

    public GameObject leftParticles;
    public GameObject rightParticles;
    public GameObject critParticles;
    private Animator anim;

    void Start() {
        player = GameMaster.instance.player.GetComponent<JPlayerController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        attackTimer = 0f;
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        comboResetTimer = 0f;
    }

    void Update() {
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }
        if (comboResetTimer > 0f) {
            comboResetTimer -= Time.deltaTime;
        }
        else {
            leftHit = false;
            rightHit = false;
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

            if (leftHit && rightHit) {
                leftHit = false;
                rightHit = false;
                comboResetTimer = 0f;
                Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 3f);
                GameObject particles = Instantiate(critParticles, transform.position, critParticles.transform.rotation);
                GameMaster.instance.player.GetComponent<JHealthController>().Heal(2);
                foreach (Collider2D col in cols) {
                    if (col.transform == transform) {
                        continue;
                    }
                    if (col.transform.tag == "Player") {
                        continue;
                    }

                    JHealthController health = col.GetComponent<JHealthController>();
                    if (health != null) {
                        health.TakeDamage(damage + 2);
                    }
                    JStatusController status = col.GetComponent<JStatusController>();
                    if (status != null) {
                        status.ApplyStun(3f);
                    }
                }
            }
            else {
                bool successfulHit = false;
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.25f, new Vector2(dir, 0f), 1f);
                foreach (RaycastHit2D hit in hits) {
                    if (hit.transform == transform) {
                        continue;
                    }
                    if (hit.transform.tag == "Player") {
                        continue;
                    }

                    JHealthController health = hit.transform.GetComponent<JHealthController>();
                    if (health != null) {
                        health.TakeDamage(damage);
                        successfulHit = true;
                        if (dir == 1) {
                            rightHit = true;
                            GameObject particles = Instantiate(rightParticles, new Vector3(transform.position.x + 0.5f, transform.position.y, 0f), Quaternion.identity) as GameObject;
                            Destroy(particles, 1f);
                        }
                        else {
                            leftHit = true;
                            GameObject particles = Instantiate(leftParticles, new Vector3(transform.position.x - 0.5f, transform.position.y, 0f), Quaternion.identity) as GameObject;
                            Destroy(particles, 1f);
                        }
                        if (comboResetTimer <= 0f) {
                            comboResetTimer = 3f;
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

                if (!successfulHit) {
                    leftHit = false;
                    rightHit = false;
                }
            }

            GameMaster.instance.inventory.UseDurability(1);
        }
    }
}
