using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleBossBehavior : MonoBehaviour, IDeathController, IGetsHurt {
    private Transform player;
    private int attackPhase = 0;
    private float attackCooldown = 3f;
    private float attackTimer;
    private bool attacking = false;
    private bool spinning = false;

    private JEnemyAIFlying ai;
    private Animator anim;
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    private float speed;
    private bool dead = false;

    private float collideCooldown = 0.1f;
    private float collideTimer;

    private AudioSource audioSource;
    public AudioClip[] audios;

    public LineRenderer lineRenderer;
    public LineRenderer laserRenderer;

    public GameObject explosionParticles;

    public ItemStack loot;
    public DroppedItem droppedItem;
    public Sprite buffIcon;
    public string buffDescription;

    void Start() {
        player = GameMaster.instance.player.transform;
        sprite = GetComponent<SpriteRenderer>();
        ai = GetComponent<JEnemyAIFlying>();
        speed = ai.speed;
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        attackTimer = 0f;
        collideTimer = 0f;
        dead = false;

        GetComponent<Collider2D>();
        StartCoroutine(Spawn());
    }

    void Update() {
        if (!attacking) {
            if (attackTimer > 0f) {
                attackTimer -= Time.deltaTime;
            }
            else {
                attackTimer = attackCooldown;
                StartCoroutine(Attack());
            }
        }
        if (collideTimer > 0f) {
            collideTimer -= Time.deltaTime;
        }


        if (ai.dir.x > 0) {
            transform.localScale = new Vector3(-1f, -1f, 1f);
        }
        else {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }


    }

    IEnumerator Spawn() {
        ai.speed = 0f;
        rb2d.velocity = Vector2.zero;
        attackTimer = 2f;
        //anim.SetTrigger("Spawn");
        yield return new WaitForSeconds(2f);
        audioSource.PlayOneShot(audios[3]);
        yield return new WaitForSeconds(1f);
        ai.speed = speed;
        attackTimer = 0f;
    }

    IEnumerator Attack() {
        attacking = true;

        float attackRange = 0f;

        if (Random.Range(0f, 1f) < 0.6f) {
            attackPhase = 1;
            attackRange = 15f;
        }
        else {
            attackPhase = 2;
            attackRange = 10f;
        }

        while (Vector3.Distance(transform.position, player.position) > attackRange) {
            yield return null;
        }

        if (attackPhase == 1) {
            StartCoroutine(LaserAttack());
            print("Attacking phase 1");
        }

        if (attackPhase == 2) {
            StartCoroutine(SpinAttack());
            print("Attacking phase 2");
        }
    }

    IEnumerator LaserAttack() {
        ai.speed = 0f;
        rb2d.velocity = Vector3.zero;

        audioSource.PlayOneShot(audios[6]);

        Vector3 pos = (player.transform.position - transform.GetChild(0).position).normalized * 30f;
        pos.z = 0f;
        lineRenderer.SetPosition(0, transform.GetChild(0).position);
        lineRenderer.SetPosition(1, pos);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.5f);
        laserRenderer.SetPosition(0, (transform.GetChild(0).position));
        laserRenderer.SetPosition(1, (transform.GetChild(0).position));
        laserRenderer.startWidth = 0.18f;
        laserRenderer.endWidth = 0.18f;
        laserRenderer.enabled = true;
        StartCoroutine(ExpandLaser(pos));

        yield return null;
    }

    Vector3 ZToTen(Vector3 v) {
        return new Vector3(v.x, v.y, 10f);
    }

    IEnumerator ExpandLaser(Vector3 pos) {
        while (laserRenderer.startWidth < 1f || Vector3.Distance(laserRenderer.GetPosition(1), (pos)) > 1f) {
            if (laserRenderer.startWidth < 1f) {
                laserRenderer.startWidth += 0.4f;
                laserRenderer.endWidth += 0.4f;
            }
            if (Vector3.Distance(laserRenderer.GetPosition(1), pos) > 1f) {
                laserRenderer.SetPosition(1, (Vector3.MoveTowards(laserRenderer.GetPosition(1), (pos), 250f * Time.deltaTime)));
            }
            yield return null;
        }

        audioSource.Stop();
        audioSource.PlayOneShot(audios[5]);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(laserRenderer.GetPosition(0), 0.5f, (laserRenderer.GetPosition(1) - laserRenderer.GetPosition(0)), 100f);
        foreach (RaycastHit2D hit in hits) {
            if (hit.transform == transform || hit.transform == transform.GetChild(3)) {
                continue;
            }
            if (hit.transform.tag == "Player") {
                GameObject particles = Instantiate(explosionParticles, hit.transform.position, explosionParticles.transform.rotation);
                particles.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                Destroy(particles, 1.5f);
                player.GetComponent<Rigidbody2D>().AddForce(new Vector2((player.transform.position - transform.position).normalized.x * 30f, 15f), ForceMode2D.Impulse);
                StartCoroutine(player.GetComponent<JPlayerController>().KnockbackDuration(0.1f));
            }
            JHealthController health = hit.transform.GetComponent<JHealthController>();
            if (health != null) {
                health.TakeDamage(5, JHealthController.DAMAGETYPE.Enemy);
            }
        }

        yield return new WaitForSeconds(0.1f);

        laserRenderer.enabled = false;
        lineRenderer.enabled = false;
        ai.speed = speed;
        attacking = false;
        attackPhase = 0;
    }

    IEnumerator SpinAttack() {
        ai.speed = 0f;
        rb2d.velocity = Vector3.zero;

        anim.SetBool("IsSpinning", true);
        audioSource.PlayOneShot(audios[2]);
        yield return new WaitForSeconds(1f);

        Vector3 pos = (player.transform.position - transform.position).normalized * 30f;
        pos.z = 0f;
        attacking = true;
        spinning = true;

        while (Vector3.Distance(pos, transform.position) > 5f) {
            transform.position = Vector3.MoveTowards(transform.position, pos, 30f * Time.deltaTime);
            yield return null;
        }

        audioSource.Stop();
        spinning = false;
        anim.SetBool("IsSpinning", false);
        ai.speed = speed;
        attacking = false;
        attackPhase = 0;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collideTimer > 0f) {
            return;
        }
        collideTimer = collideCooldown;
        JHealthController health = collision.GetComponent<JHealthController>();
        if (health != null) {
            audioSource.PlayOneShot(audios[4]);

            if (spinning) {
                health.TakeDamage(2, JHealthController.DAMAGETYPE.Melee);
            }
            else {
                health.TakeDamage(1, JHealthController.DAMAGETYPE.Melee);
            }
        }
        JStatusController status = collision.GetComponent<JStatusController>();
        if (status != null) {
            status.ApplyStun(0.5f);
        }
        Rigidbody2D rb2d = collision.GetComponent<Rigidbody2D>();
        if (rb2d != null) {
            if (spinning) {
                rb2d.AddForce(new Vector2((collision.transform.position - transform.position).normalized.x * 30f, 15f), ForceMode2D.Impulse);
            }
            else {
                rb2d.AddForce(new Vector2((collision.transform.position - transform.position).normalized.x * 20f, 10f), ForceMode2D.Impulse);
            }
        }
    }

    public void OnHurt() {
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        audioSource.PlayOneShot(audios[1]);
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
    }

    public void OnDeath() {
        if (!dead) {
            lineRenderer.enabled = false;
            laserRenderer.enabled = false;
            dead = true;
            StopAllCoroutines();
            attacking = true;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die() {
        attackTimer = 999f;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        anim.SetBool("IsAlive", false);
        audioSource.PlayOneShot(audios[0]);
        yield return new WaitForSeconds(2f);

        LevelManager.instance.bossKills++;
        DroppedItem lootClone = Instantiate(droppedItem, new Vector3(transform.position.x, transform.position.y + 1, 0f), Quaternion.identity) as DroppedItem;
        lootClone.itemStack = loot;

        if (!LevelManager.instance.centicleBuff) {
            LevelManager.instance.centicleBuff = true;
            GameMaster.instance.ShowBuffPanel(buffIcon, buffDescription);
            GameMaster.instance.playerEnergy.IncreaseMaxEnergy(50);
        }

        Destroy(gameObject);
    }
}
