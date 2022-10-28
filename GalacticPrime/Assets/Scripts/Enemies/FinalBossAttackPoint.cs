using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossAttackPoint : MonoBehaviour
{

    public int damage = 2;
    private FinalBossBehavior bossBehavior;

    public float meleeCooldown = 0.5f;
    private float meleeTimer;

    void Start()
    {
        bossBehavior = transform.parent.GetComponent<FinalBossBehavior>();
        meleeTimer = 0f;
    }

    void Update()
    {
        if (meleeTimer > 0f) {
            meleeTimer -= Time.deltaTime;
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        //print(collision.name);
        if (bossBehavior.selectedAttack == 1) {
            if (collision.tag == "Player" && meleeTimer <= 0f) {
                meleeTimer = meleeCooldown;
                JHealthController health = collision.GetComponent<JHealthController>();
                if (bossBehavior.bossStage == 1) {
                    health.TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
                }
                else {
                    health.TakeDamage(damage * 2, JHealthController.DAMAGETYPE.Enemy);
                }
                JStatusController status = collision.GetComponent<JStatusController>();
                status.ApplyStun(0.2f);
                Rigidbody2D rb2d = collision.GetComponent<Rigidbody2D>();
                rb2d.AddForce(new Vector2(-(transform.position - collision.transform.position).normalized.x * 15f, 15f), ForceMode2D.Impulse);
            }
        }
        else if (bossBehavior.charging) {
            if (collision.tag == "Player" && meleeTimer <= 0f) {
                meleeTimer = meleeCooldown;
                JHealthController health = collision.GetComponent<JHealthController>();
                health.TakeDamage(5, JHealthController.DAMAGETYPE.Enemy);
                JStatusController status = collision.GetComponent<JStatusController>();
                status.ApplyStun(1f);
                Rigidbody2D rb2d = collision.GetComponent<Rigidbody2D>();
                rb2d.AddForce(new Vector2(0f, 30f), ForceMode2D.Impulse);
            }
        }
    }
}
