using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour, IDeathController, IGetsHurt
{

    public int damage = 2;
    public float attackCooldown;
    private float attackTimer;

    private SpriteRenderer sprite;
    private Animator anim;
    private Rigidbody2D rb2d;
    private JEnemyAI ai;
    private JCharacterController character;

    public LayerMask hitLayer;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        ai = GetComponent<JEnemyAI>();
        character = GetComponent<JCharacterController>();
        attackTimer = 0f;
    }

    void Update()
    {
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }

        Vector2 attackDir = Vector2.right;
        if (transform.localScale.x < 0) {
            attackDir = Vector2.right;
        }
        else {
            if (transform.localScale.x > 0) {
                attackDir = Vector2.left;
            }
        }

        if (rb2d.velocity.y > 0.1f) {
            ai.speed = 50f;
        }
        else {
            ai.speed = 0f;
        }

        if (character.m_Grounded) {
            anim.SetBool("IsJumping", false);
        }
        else {
            anim.SetBool("IsJumping", true);
        }

        if (attackTimer <= 0f && !GetComponent<JStatusController>().stunned && character.m_Grounded) {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 1.5f, hitLayer);
            //print(hit);
            if (collider != null) {
                //print(hit.transform.name);
                StartCoroutine(Attack(collider.transform, attackDir));
            }
        }
    }

    IEnumerator Attack(Transform player, Vector2 attackDir) {
        attackTimer = attackCooldown;
        if (Vector3.Distance(player.transform.position, transform.position) <= 1.5f) {
            player.GetComponent<JHealthController>().TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2((player.transform.position - transform.position).normalized.x * 10f, 30f), ForceMode2D.Impulse);
            StartCoroutine(player.GetComponent<JPlayerController>().KnockbackDuration(0.1f));
        }
        yield return null;
    }

    public void OnDeath() {
        StartCoroutine(Die());
    }

    IEnumerator Die() {
        attackTimer = 999f;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        anim.SetBool("IsAlive", false);
        //if (audios.Length > 0) {
        //    audioSource.PlayOneShot(audios[0]);
        //}
        yield return new WaitForSeconds(1f);

        Destroy(this.gameObject);
    }

    public void OnHurt() {
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
    }
}
