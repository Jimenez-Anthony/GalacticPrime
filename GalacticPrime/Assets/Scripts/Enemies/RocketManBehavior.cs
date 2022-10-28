using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RocketManBehavior : MonoBehaviour, IDeathController, IGetsHurt 
{

    public float range;
    public float attackCooldown = 10f;
    private float attackTimer;
    private int selectedAttack = 1;
    private float attackProgressTimer;

    private SpriteRenderer sprite;
    private Animator anim;
    private AudioSource audioSource;
    private Rigidbody2D rb2d;
    private JEnemyAI groundAI;
    private JCharacterController characterCont;
    private bool walking;
    private JEnemyAIFlying flyingAI;
    private Seeker seeker;
    private bool flying;
    private Transform player;
    private JPlayerController playerCont;

    private bool dead = false;
    private bool attacking = false;

    public GameObject target;
    public RocketManRocket[] rockets;
    public BarrageRocket barrageRocket;
    public GameObject deathExplosion;
    public GameObject deathParticles;
    public AudioClip[] audios;
    public ItemStack loot;
    public DroppedItem droppedItem;

    public Sprite buffIcon;
    public string buffDescription;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
        groundAI = GetComponent<JEnemyAI>();
        characterCont = GetComponent<JCharacterController>();
        flyingAI = GetComponent<JEnemyAIFlying>();
        seeker = GetComponent<Seeker>();
        player = GameMaster.instance.player.transform;
        playerCont = player.GetComponent<JPlayerController>();
        walking = true;
        flying = false;
        flyingAI.enabled = false;

        attackTimer = 0f;
        dead = false;
        attacking = false;
    }

    void Update()
    {
        if (attackTimer > 0f && !attacking) {
            attackTimer -= Time.deltaTime;
        }
        else {
            if (!attacking && Vector3.Distance(transform.position, player.position) < range) {
                Attack();
            }
        }

        if (attackProgressTimer > 0f) {
            attackProgressTimer -= Time.deltaTime;
        }
        else {
            selectedAttack = 0;
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    void Attack() {
        attacking = true;
        attackTimer = attackCooldown;
        if (selectedAttack == 0) {
            if (Random.Range(0f, 1f) < 0.5f) {
                selectedAttack = 1;
                attackProgressTimer = 10f + Random.Range(-2f, 6f);
                rb2d.velocity = new Vector2(0f, 5f);
                StartCoroutine(LongRangeAttack());
            }
            else {
                selectedAttack = 2;
                attackProgressTimer = 8f + Random.Range(-2f, 2f);
                //audioSource.PlayOneShot(audios[1]);
                audioSource.clip = audios[1];
                audioSource.Play();
                audioSource.loop = true;
                StartCoroutine(ShortRangeAttack());
            }
        }
        else {
            if (selectedAttack == 1) {
                StartCoroutine(LongRangeAttack());
            }
            else {
                StartCoroutine(ShortRangeAttack());
            }
        }
    }

    IEnumerator LongRangeAttack() {
        SwitchAI(1);
        yield return new WaitForSeconds(1);
        float speed = flyingAI.speed;
        flyingAI.speed = 0f;
        rb2d.velocity = Vector2.zero;
        Vector3 firstTargetLoc = player.transform.position;
        GameObject firstTarget = Instantiate(target, firstTargetLoc, Quaternion.identity) as GameObject;
        Destroy(firstTarget, 1.5f);
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        Vector3 secondTargetLoc;
        if (Vector3.Distance(player.position, firstTargetLoc) > 2f) {
            secondTargetLoc = player.position;
        }
        else {
            if (playerCont.facingRight) {
                secondTargetLoc = new Vector3(firstTargetLoc.x + 2f, player.position.y, 0f);
            }
            else {
                secondTargetLoc = new Vector3(firstTargetLoc.x - 2f, player.position.y, 0f);
            }
        }
        GameObject secondTarget = Instantiate(target, secondTargetLoc, Quaternion.identity) as GameObject;
        Destroy(secondTarget, 1.5f);

        yield return new WaitForSeconds(0.1f);
        RocketManRocket rocket1 = Instantiate(rockets[0], transform.GetChild(1).position, Quaternion.identity) as RocketManRocket;
        audioSource.PlayOneShot(audios[2]);
        rocket1.targetLoc = firstTargetLoc;
        yield return new WaitForSeconds(0.3f);
        RocketManRocket rocket2 = Instantiate(rockets[1], transform.GetChild(2).position, Quaternion.identity) as RocketManRocket;
        audioSource.PlayOneShot(audios[2]);
        rocket2.targetLoc = secondTargetLoc;

        attacking = false;
        flyingAI.speed = speed;
        if (selectedAttack == 1) {
            attackTimer = 0.1f;
        }
    }

    IEnumerator ShortRangeAttack() {
        SwitchAI(0);

        int dir = 1;
        if (transform.localScale.x > 0) {
            dir = -1;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.GetChild(3).position, new Vector3(dir, 0f), 2.5f);
        //audioSource.Pause();
        foreach (RaycastHit2D hit in hits) {
            if (hit.transform.tag == "Player") {
                //anim.SetTrigger("Attack");
                ////audioSource.UnPause();
                //BarrageRocket rocket = Instantiate(barrageRocket, new Vector3(transform.GetChild(3).position.x, transform.GetChild(3).position.y + Random.Range(-0.3f, 0.3f), 0f), Quaternion.identity) as BarrageRocket;
                //rocket.dir = dir;
            }
        }
        anim.SetTrigger("Attack");
        //audioSource.UnPause();
        BarrageRocket rocket = Instantiate(barrageRocket, new Vector3(transform.GetChild(3).position.x, transform.GetChild(3).position.y + Random.Range(-0.3f, 0.3f), 0f), Quaternion.identity) as BarrageRocket;
        rocket.dir = dir;

        attacking = false;
        if (selectedAttack == 2) {
            attackTimer = 0.1f;
        }

        yield return null;
    }

    void SwitchAI(int num) {
        print("Switching ai to " + num);
        if (num == 0) {
            flying = false;
            walking = true;
            anim.SetBool("IsFlying", false);
            groundAI.enabled = true;
            characterCont.enabled = true;
            flyingAI.enabled = false;
            seeker.enabled = false;
            rb2d.gravityScale = 2f;
        }
        else {
            walking = false;
            flying = true;
            audioSource.PlayOneShot(audios[0]);
            anim.SetBool("IsFlying", true);
            groundAI.enabled = false;
            characterCont.enabled = false;
            flyingAI.enabled = true;
            seeker.enabled = true;
            rb2d.gravityScale = 0;
        }
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
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die() {
        attackTimer = 999f;
        attacking = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        anim.SetBool("IsAlive", false);
        audioSource.PlayOneShot(audios[0]);
        yield return new WaitForSeconds(0.6f);

        sprite.enabled = false;
        audioSource.enabled = false;

        GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, 0.5f);
        GameObject particles = Instantiate(deathParticles, transform.position, Quaternion.identity) as GameObject;
        Destroy(particles, 1f);

        yield return new WaitForSeconds(0.5f);

        DroppedItem lootClone = Instantiate(droppedItem, new Vector3(transform.position.x, transform.position.y + 1, 0f), Quaternion.identity) as DroppedItem;
        lootClone.itemStack = loot;
        LevelManager.instance.bossKills++;

        if (!LevelManager.instance.rocketManBuff) {
            LevelManager.instance.rocketManBuff = true;
            GameMaster.instance.ShowBuffPanel(buffIcon, buffDescription);
            GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(5);
            GameMaster.instance.player.GetComponent<JHealthController>().environmentalImmune = true;
        }

        Destroy(gameObject);
    }
}
