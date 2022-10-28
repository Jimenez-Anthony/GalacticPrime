using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FinalBossBehavior : MonoBehaviour, IDeathController, IGetsHurt {


    public int bossStage = 1;

    public float attackCooldown = 10f;
    private float attackTimer;
    private bool attacking = true;
    public int selectedAttack = 0;

    private Transform aiTarget;
    private float scale;
    private int dir;

    private bool dead;
    private Animator anim;
    private SpriteRenderer sprite;
    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private JEnemyAI groundAI;
    private JCharacterController characterCont;
    private bool walking;
    private JEnemyAIFlying flyingAI;
    private Seeker seeker;
    private bool flying;
    private Transform player;
    private JPlayerController playerCont;
    private JHealthController health;

    public GameObject[] reinfocrementsOne;
    public GameObject[] reinforcementsTwo;
    public GameObject[] reinforcementsThree;

    // Particles
    public GameObject spawnParticles;

    // Tools
    public FinalBossFireball fireBall;
    public FinalBossGrenade grenade;
    public bool charging = false;
    public GameObject target;
    public RocketManRocket[] rockets;
    public GameObject meteor;
    public GameObject summonedBoss;

    public AudioClip[] audios;

    void Start()
    {
        bossStage = 1;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        scale = transform.localScale.x;
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        groundAI = GetComponent<JEnemyAI>();
        characterCont = GetComponent<JCharacterController>();
        flyingAI = GetComponent<JEnemyAIFlying>();
        seeker = GetComponent<Seeker>();
        player = GameMaster.instance.player.transform;
        playerCont = player.GetComponent<JPlayerController>();
        health = transform.GetChild(5).GetComponent<JHealthController>();
        dead = false;
        walking = true;
        flying = false;
        flyingAI.enabled = false;
        selectedAttack = 0;
         //scale = transform.localScale.x;


        aiTarget = Instantiate(new GameObject(), player.transform.position, Quaternion.identity).transform;
        attackTimer = 0f;
        attacking = false;
        charging = false;
        SwitchAI(0);
    }

    void Update()
    {
        if (!flying) {
            dir = -1;
            if (transform.localScale.x > 0) {
                dir = 1;
            }
        }

        int moveDir = 1;
        if (player.position.x - transform.position.x < 0) {
            moveDir = -1;
        }

        dir = 1;
        if (player.transform.position.x > transform.position.x) {
            dir = -1;
        }

        if (dir > 0) {
            transform.localScale = new Vector3(scale, transform.localScale.y, 1);
        }
        else {
            transform.localScale = new Vector3(-scale, transform.localScale.y, 1);

        }

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + dir * 1, transform.position.y - 1), new Vector2(moveDir, 0), 1.5f, LayerMask.NameToLayer("Obstacle"));
        if (walking && !hit && characterCont.m_Grounded) {
            //characterCont.Move(dir * Time.fixedDeltaTime * groundAI.speed, true, player.transform);
        }

        if (!attacking) {
            if (attackTimer > 0f) {
                attackTimer -= Time.deltaTime;
            }
            else {
                Attack();
            }
        }

        if (walking) {
            RaycastHit2D groundHit = Physics2D.Raycast(transform.GetChild(0).position, new Vector2(moveDir, 0f), 3f, LayerMask.GetMask("Obstacle"));
            if (groundHit) {
                characterCont.Move(moveDir * groundAI.speed * Time.deltaTime, true, player);
            }
            else {
                characterCont.Move(moveDir * groundAI.speed * Time.deltaTime, false, player);
            }
        }

        if (flying && !attacking) {
            aiTarget.position = new Vector3(player.transform.position.x, player.transform.position.y + 5f, 0f);
        }

        if (Mathf.Abs(rb2d.velocity.x) < 0.1f) {
            anim.SetBool("IsWalking", false);
        }
        else {
            anim.SetBool("IsWalking", true);
        }

        if (bossStage == 1 && health.hp < 800) {
            NextStage(2);
        }
        if (bossStage == 2 && health.hp < 300) {
            NextStage(3);
        }

    }

    void NextStage(int stage) {
        print("Boss entering stage " + stage);
        bossStage = stage;
    }

    void Attack() {
        attacking = true;
        attackTimer = attackCooldown;
        if (bossStage == 1) {
            if (Random.Range(0f, 1f) < 0.25f) {
                StartCoroutine(MeleeAttack(Random.Range(3f, 6f)));
            }
            else {
                StartCoroutine(SummonReinforcement((int)Random.Range(1f, 3.1f)));
            }
        }
        if (bossStage == 2) {
            float num = Random.Range(0f, 1.2f);
            if (num < 0.1f) {
                //StartCoroutine(FireBall((int)Random.Range(1f, 5.1f)));
                //StartCoroutine(ThrowGrenades(3));
                //StartCoroutine(ChargeAttack());
                StartCoroutine(RocketFury());
                //StartCoroutine(SummonMeteor());
            }
            else if (num < 0.3f) {
                StartCoroutine(ChargeAttack());
            }
            else if (num < 0.5f) {
                StartCoroutine(ThrowGrenades(3));
            }
            else if (num < 0.7f) {
                StartCoroutine(FireBall((int)Random.Range(1f, 5.1f)));
            }
            else if (num < 1f) {
                StartCoroutine(MeleeAttack(Random.Range(1f, 5f)));
            }
            else {
                StartCoroutine(SummonReinforcement((int)Random.Range(2f, 5.1f)));
            }
        }
        if (bossStage == 3) {
            float num = Random.Range(0f, 0.9f);
            if (num < 0.1f) {
                StartCoroutine(SummonMeteor());
            }
            else if (num < 0.2f) {
                StartCoroutine(SummonReinforcement(1));
            }
            else if (num < 0.5f) {
                StartCoroutine(RocketFury());
            }
            else {
                StartCoroutine(FireBall((int)Random.Range(4f, 7.1f)));
            }
        }
    }

    IEnumerator MeleeAttack(float duration) {
        print("Melee attack");
        SwitchAI(0);
        selectedAttack = 1;
        yield return new WaitForSeconds(duration);
        selectedAttack = 0;
        attacking = false;
    }

    IEnumerator SummonReinforcement(int count) {
        print("Summoning reinforcements");
        selectedAttack = 2;
        float speed = groundAI.speed;
        groundAI.speed = 0f;

        audioSource.PlayOneShot(audios[3]);
        yield return new WaitForSeconds(2f);

        groundAI.speed = speed;
        List<Vector3> spots = new List<Vector3>();
        while (spots.Count < count) {
            float minDistance = 9999f;
            Vector3 closest = Vector3.zero;
            foreach (Vector3 loc in CityGenerator.buildingLocations) {
                if (!spots.Contains(loc) && Vector3.Distance(transform.position, loc) < minDistance) {
                    minDistance = Vector3.Distance(transform.position, loc);
                    closest = loc;
                }
            }
            spots.Add(closest);
        }

        foreach (Vector3 loc in spots) {
            GameObject particles = Instantiate(spawnParticles, loc, Quaternion.identity) as GameObject;
            Destroy(particles, 1f);
            GameObject summon = null;
            if (bossStage == 1) {
                summon = reinfocrementsOne[(int)Random.Range(0f, reinfocrementsOne.Length)];
            }
            else if (bossStage == 2) {
                summon = reinforcementsTwo[(int)Random.Range(0f, reinforcementsTwo.Length)];
            }
            else if (bossStage == 3) {
                summon = reinforcementsThree[(int)Random.Range(0f, reinforcementsThree.Length)];
            }

            if (bossStage == 3) {
                if (summonedBoss == null) {
                    GameObject clone = Instantiate(summon, loc, Quaternion.identity) as GameObject;
                    summonedBoss = clone;
                }
            }
            else {
                GameObject clone = Instantiate(summon, loc, Quaternion.identity) as GameObject;
            }
        }

        selectedAttack = 0;
        attacking = false;
    }

    IEnumerator FireBall(int count) {
        print("Fireball attack");
        selectedAttack = 3;
        SwitchAI(1);
        flyingAI.target = aiTarget;
        yield return new WaitForSeconds(2f);

        float speed = flyingAI.speed;
        flyingAI.speed = 0f;
        rb2d.velocity = Vector2.zero;

        for (int i = 0; i < count; i++) {
            FinalBossFireball ball = Instantiate(fireBall, transform.GetChild(2).position, Quaternion.identity);
            ball.transform.SetParent(transform.GetChild(2));
            audioSource.PlayOneShot(audios[2]);
            yield return new WaitForSeconds(1.1f);
            if (ball != null) {
                ball.transform.SetParent(null);
                ball.faceDir = dir;
                ball.targetLoc = (new Vector3(player.transform.position.x, player.transform.position.y, 0f) - transform.GetChild(2).position).normalized * 50f;
                ball.targetSelected = true;
            }
        }

        flyingAI.speed = speed;
        selectedAttack = 0;
        attackTimer = 0f;
        attacking = false;
    }

    IEnumerator ThrowGrenades(int count) {
        print("Grenades attack");
        selectedAttack = 4;
        SwitchAI(1);
        flyingAI.target = aiTarget;

        List<Vector3> spots = new List<Vector3>();
        while (spots.Count < count) {
            float minDistance = 9999f;
            Vector3 closest = Vector3.zero;
            foreach (Vector3 loc in CityGenerator.buildingLocations) {
                if (!spots.Contains(loc) && Vector3.Distance(player.transform.position, loc) < minDistance) {
                    minDistance = Vector3.Distance(player.transform.position, loc);
                    closest = loc;
                }
            }
            spots.Add(closest);
        }

        float speed = flyingAI.speed;
        rb2d.velocity = Vector2.zero;
        flyingAI.speed = 0f;

        foreach (Vector3 loc in spots) { 
            aiTarget.position = new Vector3(loc.x, loc.y + 15f);
            //flyingAI.speed = 70f; 
            while (Vector3.Distance(aiTarget.position, transform.position) > 2f) {
                //flyingAI.target = aiTarget;
                transform.position = Vector3.MoveTowards(transform.position, aiTarget.position, Time.deltaTime * 70f);
                yield return 0;
            }

            anim.SetTrigger("Throw");
            audioSource.PlayOneShot(audios[0]);
            yield return new WaitForSeconds(0.1f);
            GameObject clone = Instantiate(grenade, transform.GetChild(3).position, Quaternion.identity).gameObject;
            Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), transform.GetChild(5).GetComponent<Collider2D>());
            //spots.Remove(spots[0]);
        }

        flyingAI.speed = speed;

        attacking = false;
        selectedAttack = 0;

    }

    IEnumerator ChargeAttack() {
        print("Charge Attack");
        selectedAttack = 5;
        SwitchAI(0);

        yield return new WaitForSeconds(0.5f);

        walking = false;
        rb2d.velocity = Vector2.zero;

        transform.GetChild(4).GetChild(0).GetComponent<TrailRenderer>().enabled = true;
        transform.GetChild(4).GetChild(1).GetComponent<TrailRenderer>().enabled = true;
        transform.GetChild(4).GetChild(2).GetComponent<TrailRenderer>().enabled = true;

        yield return new WaitForSeconds(1f);

        Vector3 chargeDest = new Vector3(transform.position.x - dir * 30f, transform.position.y, 0f);
        SwitchAI(1);
        float speed = flyingAI.speed;
        flyingAI.speed = 0f;
        charging = true;
        while (Vector3.Distance(transform.position, chargeDest) > 1f) {
            transform.position = Vector3.MoveTowards(transform.position, chargeDest, Time.deltaTime * 70f);
            yield return 0;
        }
        flyingAI.speed = speed;

        transform.GetChild(4).GetChild(0).GetComponent<TrailRenderer>().enabled = false;
        transform.GetChild(4).GetChild(1).GetComponent<TrailRenderer>().enabled = false;
        transform.GetChild(4).GetChild(2).GetComponent<TrailRenderer>().enabled = false;

        while (transform.position.y < player.position.y) {
            yield return new WaitForSeconds(0.3f);
        }

        attacking = false;
        charging = false;
        selectedAttack = 0;
    }

    IEnumerator RocketFury() {
        print("Rocket Furry");
        selectedAttack = 6;
        SwitchAI(1);

        yield return new WaitForSeconds(0.5f);

        rb2d.velocity = Vector2.zero;
        float speed = flyingAI.speed;
        flyingAI.speed = 0f;

        Vector3 position = player.transform.position;

        audioSource.PlayOneShot(audios[1]);

        for (int x = (int)position.x - 15; x < (int)position.x + 15; x += 4) {
            for (int y = (int)position.y - 10; y < (int)position.y + 10; y += 4) {
                GameObject targetClone = Instantiate(target, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                Destroy(targetClone, 1.5f);
                StartCoroutine(FireRocket(new Vector3(x, y, 0f)));
            }
            yield return 0;
        }

        yield return new WaitForSeconds(1f);

        flyingAI.speed = speed;
        attacking = false;
        selectedAttack = 0;
    }

    IEnumerator FireRocket(Vector3 loc) {
        yield return new WaitForSeconds(1.5f);
        RocketManRocket rocketClone = Instantiate(rockets[(int)Random.Range(0.5f, 1.5f)], transform.GetChild(2).position, Quaternion.identity);
        rocketClone.targetLoc = loc;
    }

    void SwitchAI(int num) {
        print("Switching ai to " + num);
        if (num == 0) {
            flying = false;
            walking = true;
            //anim.SetBool("IsFlying", false);
            //groundAI.enabled = true;
            characterCont.enabled = true;
            flyingAI.enabled = false;
            seeker.enabled = false;
            rb2d.gravityScale = 4f;
            GetComponent<Collider2D>().isTrigger = false;
        }
        else {
            walking = false;
            flying = true;
            //audioSource.PlayOneShot(audios[0]);
            //anim.SetBool("IsFlying", true);
            groundAI.enabled = false;
            characterCont.enabled = false;
            flyingAI.enabled = true;
            seeker.enabled = true;
            rb2d.gravityScale = 0;
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    IEnumerator SummonMeteor() {
        print("Summoning meteor");
        SwitchAI(1);
        float speed = flyingAI.speed;
        flyingAI.speed = 0f;
        yield return new WaitForSeconds(1f);

        //anim.SetTrigger()

        List<Vector3> spots = new List<Vector3>();
        while (spots.Count < 1) {
            float minDistance = 9999f;
            Vector3 closest = Vector3.zero;
            foreach (Vector3 loc in CityGenerator.buildingLocations) {
                if (!spots.Contains(loc) && Vector3.Distance(player.transform.position, loc) < minDistance) {
                    minDistance = Vector3.Distance(player.transform.position, loc);
                    closest = loc;
                }
            }
            spots.Add(closest);
        }

        Instantiate(meteor, new Vector3(spots[0].x, spots[0].y + 30, 0), meteor.transform.rotation);
        CityGenerator.buildingLocations.Remove(spots[0]);
        yield return new WaitForSeconds(0.5f);
        flyingAI.speed = speed;
        attacking = false;
        selectedAttack = 0;
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

        if (summonedBoss != null) {
            Destroy(summonedBoss.gameObject);
        }

        AudioManager.instance.StopSounds();
        AudioManager.instance.Play("Victory");

        //audioSource.PlayOneShot(audios[0]);
        yield return new WaitForSeconds(3f);

        CityGenerator.instance.OnBossKilled();
        AudioManager.instance.Play("Fireworks");
        sprite.enabled = false;
        //audioSource.enabled = false;
        Destroy(gameObject);
    }
}
