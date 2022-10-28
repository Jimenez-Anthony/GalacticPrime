using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMechBehavior : MonoBehaviour, IDeathController, IGetsHurt {

    public int damage = 2;
    public float attackCooldown = 3f;
    public float attackRange = 1f;
    public float attackKnockback = 100f;
    private float attackTimer;
    public LayerMask hitLayer;

    private bool dead = false;

    private AudioSource audioSource;
    public AudioClip[] audios;

    JEnemyAIFlying ai;
    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb2d;

    void Start() {
        ai = GetComponent<JEnemyAIFlying>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
        attackTimer = 0f;
    }

    void Update() {
        if (attackTimer > 0) {
            attackTimer -= Time.deltaTime;
        }
        //if (rb2d.velocity.x > 0.1f) {
        //    audioSource.Play();
        //}
        //else {
        //    audioSource.Stop();
        //}

        if (ai.dir.x > 0) {
            transform.localScale = new Vector3(-1f, -1f, 1f);
        }
        else {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        Vector2 attackDir = Vector2.right;
        if (transform.localScale.x < 0) {
            //print("attacking right");
            attackDir = Vector2.right;
        }
        else {
            if (transform.localScale.x > 0) {
                //print("attacking left");
                attackDir = Vector2.left;
            }
        }

        if (attackTimer <= 0f) {
            RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).position, attackDir, attackRange, hitLayer);
            //print(hit);
            if (hit != false) {
                //print(hit.transform.name);
                StartCoroutine(Attack(hit.transform, attackDir));
            }
        }
    }

    IEnumerator Attack(Transform player, Vector2 attackDir) {
        //print("Snake attacking");
        attackTimer = attackCooldown;
        anim.SetTrigger("IsAttacking");
        //AudioManager.instance.Play("SkeletonAttack");

        float speed = GetComponent<JEnemyAIFlying>().speed;
        GetComponent<JEnemyAIFlying>().speed = 0f;
        rb2d.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.4f);
        audioSource.PlayOneShot(audios[2]);
        if (Vector3.Distance(player.transform.position, transform.position) <= 0.75f * 2) {
            player.GetComponent<JHealthController>().TakeDamage(damage);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2((player.transform.position - transform.position).normalized.x * attackKnockback, 0f), ForceMode2D.Impulse);
            StartCoroutine(player.GetComponent<JPlayerController>().KnockbackDuration(0.1f));
            //player.GetComponent<JStatusController>().ApplySlow(1f, 0.99f);
            //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, -100f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.1f);
        GetComponent<JEnemyAIFlying>().speed = speed;
        //anim.SetBool("IsAttacking", false);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.GetChild(0).position, new Vector2(transform.position.x + attackRange, transform.position.y));
    }

    public void OnDeath() {
        if (!dead) {
            dead = true;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die() {
        attackTimer = 999f;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("Die");
        //AudioManager.instance.Play("SkeletonDeath");
        audioSource.PlayOneShot(audios[0]);
        yield return new WaitForSeconds(0.6f);
        Destroy(this.gameObject);
    }

    public void OnHurt() {
        //GetComponent<Rigidbody2D>().AddForce(new Vector2((transform.position - GameMaster.instance.player.transform.position).normalized.x * 10f, 3f), ForceMode2D.Impulse);
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        //AudioManager.instance.Play("SkeletonHit");
        audioSource.PlayOneShot(audios[1]);
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
    }
}