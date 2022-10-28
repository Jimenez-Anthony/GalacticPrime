using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBehavior : MonoBehaviour, IDeathController, IGetsHurt
{
    private float range = 15f;
    public int damage = 3;
    public float attackCooldown = 2.5f;
    private float attackTimer;

    public MageShot bullet;
    public AudioClip[] audios;

    private int dir;
    private bool dead = false;

    private Animator anim;
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        attackTimer = 1f;
        dead = false;
    }

    void Update()
    {
        if (GameMaster.instance.player.transform.position.x - transform.position.x > 0) {
            dir = 1;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else {
            dir = -1;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }
        else {
            if (Vector3.Distance(GameMaster.instance.player.transform.position, transform.position) < range) {
                attackTimer = attackCooldown;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack() {
        anim.SetTrigger("Attack");
        audioSource.PlayOneShot(audios[1]);
        yield return new WaitForSeconds(0.35f);
        MageShot clone = Instantiate(bullet, transform.GetChild(0).position, bullet.transform.rotation) as MageShot;
        clone.faceDir = dir;
        clone.damage = damage;
        clone.targetLoc = GameMaster.instance.player.transform.position;
    }

    public void OnHurt() {
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        //audioSource.PlayOneShot(audios[1]);
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
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
        anim.SetBool("IsAlive", false);
        audioSource.PlayOneShot(audios[0]);
        yield return new WaitForSeconds(0.9f);
        Destroy(this.gameObject);
    }
}
