using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour, IDeathController, IGetsHurt
{

    public int damage = 1;
    public float attackCooldown = 1.5f;
    public float attackRange = 1f;
    public float attackKnockback = 100f;
    private float attackTimer;
    public LayerMask hitLayer;

    public bool poison = false;
    public int poisonDamage = 1;
    public float poisonDuration = 3f;
    public GameObject poisonParticles;
    private GameObject previousParticles;

    private AudioSource audioSource;
    public AudioClip[] audios;

    JEnemyAI ai;
    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb2d;

    void Start()
    {
        ai = GetComponent<JEnemyAI>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
        attackTimer = 0f;
    }

    void Update()
    {
        if (attackTimer > 0) {
            attackTimer -= Time.deltaTime;
        }
        if (rb2d.velocity.x > 0f) {
            //audioSource.PlayOneShot(audios[2]);
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

        if (attackTimer <= 0f && !GetComponent<JStatusController>().stunned) {
            RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(1).position, attackDir, attackRange, hitLayer);
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
        anim.SetBool("IsAttacking", true);
        if (audios.Length > 0) {
            audioSource.PlayOneShot(audios[1]);
        }
        yield return new WaitForSeconds(0.4f);

        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange) {
            player.GetComponent<JHealthController>().TakeDamage(damage, JHealthController.DAMAGETYPE.Enemy);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2((player.transform.position - transform.position).normalized.x * attackKnockback, 10f), ForceMode2D.Impulse);
            StartCoroutine(player.GetComponent<JPlayerController>().KnockbackDuration(0.1f));
            if (poison) {
                player.GetComponent<JStatusController>().ApplyPoison(1, poisonDuration, poisonDamage, false);
                if (previousParticles != null) {
                    Destroy(previousParticles);
                }
                GameObject particles = Instantiate(poisonParticles, GameMaster.instance.player.transform.GetChild(0), false) as GameObject;
                Destroy(particles, poisonDuration);
                previousParticles = particles;
            }
        }

        yield return new WaitForSeconds(0.4f);
        anim.SetBool("IsAttacking", false);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + attackRange, transform.position.y));
    }

    public void OnDeath() {
        StartCoroutine(Die());
    }

    IEnumerator Die() {
        attackTimer = 999f;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        anim.SetBool("IsAlive", false);
        if (audios.Length > 0) {
            audioSource.PlayOneShot(audios[0]);
        }

        // Gordon
        if (GetComponent<JHealthController>().maxHP != 2) {
            yield return new WaitForSeconds(1.1f);
        }
        Destroy(this.gameObject);
    }

    public void OnHurt() {
        //GetComponent<Rigidbody2D>().AddForce(new Vector2((transform.position - GameMaster.instance.player.transform.position).normalized.x * 10f, 3f), ForceMode2D.Impulse);
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
    }
}
