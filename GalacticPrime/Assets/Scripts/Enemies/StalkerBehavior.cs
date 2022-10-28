using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerBehavior : MonoBehaviour, IDeathController, IGetsHurt
{

    public float range = 10f;
    public float attackCooldown = 0.5f;
    private float attackTimer;
    private bool attacking;
    public float tpCooldown = 0.2f;
    private float tpTimer;
    private bool dead;
    private int unsuccessfulHits = 0;

    private float healCooldown = 0.25f;
    private float healTimer;

    private Transform player;
    private JPlayerController playerCont;
    private SpriteRenderer sprite;
    private Animator anim;
    private Rigidbody2D rb2d;
    private JHealthController healthCont;
    private AudioSource audioSource;

    public bool ballLanded;
    public Vector3 ballLandingLocation;

    private bool facedRight = false;
    private bool teleporting = false;

    public GameObject teleportParticles;
    public StalkerBall ball;
    public DroppedItem droppedItem;
    public ItemStack loot;

    public AudioClip[] audio;

    public Sprite buffIcon;
    public string buffDescription;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        healthCont = GetComponent<JHealthController>();
        healthCont.environmentalImmune = true;
        player = GameMaster.instance.player.transform;
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        attackTimer = 0f;
        tpTimer = 0f;
        healTimer = 0f;
        unsuccessfulHits = 0;
        dead = false;
        attacking = false;
        ballLanded = false;
    }

    void Update()
    {
        if (healTimer > 0f) {
            healTimer -= Time.deltaTime;
        }

        if (Vector3.Distance(player.position, transform.position) > 15f) {
            if (healTimer <= 0f) {
                healthCont.Heal(1);
                healTimer = healCooldown;
            }
            healthCont.invulnerable = true;
        }
        else {
            healthCont.invulnerable = false;
        }


        if (attackTimer > 0f && !attacking) {
            attackTimer -= Time.deltaTime;
        }
        else if (Vector3.Distance(player.position, transform.position) < 0.5f && !teleporting) {
            attackTimer = attackCooldown;
            if (Random.Range(0f, 1f) > 0.75f) {
                //print("Attacking ranged");
                StartCoroutine(RangedAttack());
            }
            else {
                StartCoroutine(Attack());
            }
        }
        else if (unsuccessfulHits > 5 && !teleporting) {
            attackTimer = attackCooldown;
            unsuccessfulHits = 0;
            StartCoroutine(RangedAttack());
        }

        if (tpCooldown > 0f) {
            tpCooldown -= Time.deltaTime;
        }

        if (Vector3.Distance(player.position, transform.position) < 0.5f && playerCont.facingRight != facedRight && !teleporting && !attacking) {
            attackTimer = attackCooldown;
            if (tpCooldown <= 0f) {
                StartCoroutine(TeleportToPlayer());
            }
        }

        if (Vector3.Distance(player.position, transform.position) < range && Vector3.Distance(player.position, transform.position) > 0.5f && !teleporting && !attacking) {
            if (tpCooldown <= 0f) {
                StartCoroutine(TeleportToPlayer());
            }
        }

        if (player.position.x - transform.position.x > 0) {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        facedRight = playerCont.facingRight;
    }

    IEnumerator Attack() {
        anim.SetTrigger("Attack");
        player.GetComponent<JHealthController>().TakeDamage(1, JHealthController.DAMAGETYPE.Enemy);
        player.GetComponent<JStatusController>().ApplyStun(0.25f);
        unsuccessfulHits = 0;
        if (player.position.x - transform.position.x > 0) {
            //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(10f, 4f), ForceMode2D.Impulse);
        }
        else {
            //player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10f, 4f), ForceMode2D.Impulse);
        }
        yield return null;
    }

    IEnumerator RangedAttack() {
        attacking = true;
        Vector3 teleLoc = Vector3.zero;
        int dir = 0;
        if (playerCont.facingRight) {
            dir = -1;
            teleLoc = new Vector3(player.transform.position.x + 5f, player.transform.position.y, 0f);
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, 0f), (teleLoc - transform.position).normalized, 5f, LayerMask.NameToLayer("Obstacle"));
            if (hit) {
                //print("hit obstacle");
                teleLoc = new Vector3(player.transform.position.x + hit.distance, player.transform.position.y, 0f);
            }
        }
        else {
            dir = 1;
            teleLoc = new Vector3(player.transform.position.x - 5f, player.transform.position.y, 0f);
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, 0f), (teleLoc - transform.position).normalized, 5f, LayerMask.NameToLayer("Obstacle"));
            if (hit) {
                //print("hit obstacle");
                teleLoc = new Vector3(player.transform.position.x - hit.distance, player.transform.position.y, 0f);
            }
        }

        StartCoroutine(TeleportToLocation(teleLoc));

        while (teleporting) {
            yield return null;
        }

        StalkerBall ballClone = Instantiate(ball, new Vector3(transform.position.x + dir, transform.position.y, 0f), Quaternion.identity) as StalkerBall;
        ballClone.faceDir = dir;
        ballClone.stalker = this;
        audioSource.PlayOneShot(audio[0]);
        yield return new WaitForSeconds(1f);

        attacking = false;
    }

    IEnumerator TeleportToPlayer() {
        Vector3 teleLoc = Vector3.zero;
        teleporting = true;
        unsuccessfulHits++;

        anim.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.15f);
        GameObject clone = Instantiate(teleportParticles, transform.position, teleportParticles.transform.rotation);
        Destroy(clone, 1f);

        if (playerCont.facingRight) {
            teleLoc = new Vector3(player.transform.position.x - 0.4f, player.transform.position.y, 0f);
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, 0f), (teleLoc - transform.position).normalized, 0.4f, LayerMask.NameToLayer("Obstacle"));
            if (hit) {
                teleLoc = new Vector3(player.transform.position.x - hit.distance, player.transform.position.y, 0f);

            }

        }
        else {
            teleLoc = new Vector3(player.transform.position.x + 0.4f, player.transform.position.y, 0f);
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, 0f), (teleLoc - transform.position).normalized, 0.4f, LayerMask.NameToLayer("Obstacle"));
            if (hit) {
                teleLoc = new Vector3(player.transform.position.x + hit.distance, player.transform.position.y, 0f);

            }
        }

        if (ballLanded) {
            teleLoc = ballLandingLocation;
            attackTimer = 2.5f;
            tpCooldown = 3f;
        }

        transform.position = teleLoc;

        anim.SetTrigger("Arrive");
        yield return new WaitForSeconds(0.15f);
        teleporting = false;

        if (!ballLanded) {
            tpCooldown = tpTimer;
        }
    }

    IEnumerator TeleportToLocation(Vector3 teleLoc) {
        teleporting = true;

        anim.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.15f);
        GameObject clone = Instantiate(teleportParticles, transform.position, teleportParticles.transform.rotation);
        Destroy(clone, 1f);

        transform.position = teleLoc;

        anim.SetTrigger("Arrive");
        yield return new WaitForSeconds(0.15f);
        teleporting = false;
        tpCooldown = tpTimer;
    }

    public void OnHurt() {
        StartCoroutine(OnHurtAnim());
    }

    IEnumerator OnHurtAnim() {
        //audioSource.PlayOneShot(audios[1]);
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color32(255, 122, 122, 255);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color32(255, 255, 255, 255);
    }

    public void OnDeath() {
        if (!dead) {
            dead = true;
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die() {
        attackTimer = 999f;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        //anim.SetBool("IsAlive", false);
        //audioSource.PlayOneShot(audios[0]);
        yield return new WaitForSeconds(0.5f);

        DroppedItem lootClone = Instantiate(droppedItem, new Vector3(transform.position.x, transform.position.y + 1, 0f), Quaternion.identity) as DroppedItem;
        lootClone.itemStack = loot;
        LevelManager.instance.bossKills++;

        if (!LevelManager.instance.stalkerBuff) {
            LevelManager.instance.stalkerBuff = true;
            GameMaster.instance.ShowBuffPanel(buffIcon, buffDescription);
            GameMaster.instance.player.GetComponent<Dash>().dashCooldown = 1;
        }

        Destroy(gameObject);
    }
}
