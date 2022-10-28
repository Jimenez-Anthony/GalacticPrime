using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JStatusController : MonoBehaviour
{

    public bool ai = false;
    public bool overrideS = false;
    public float overrideSpeed = 0f;
    public bool stunned = false;

    private JPlayerController playerCont;
    private JEnemyAI enemyAI;
    private JEnemyAIFlying enemyFlyingAI;

    private JHealthController healthCont;

    public GameObject stunParticles;
    public GameObject invulnerabilityParticles;
    //public GameObject poisonParticles;

    float normalSpeed = 0f;

    // Slow
    private List<SpeedMod> slowFactors;
    float slowDuration = 0f;
    float slowPercentage = 0f;

    // Posion
    private List<DotMod> dotFactors;
    private float dotTimer;

    // Wither
    public float witherTimer = 0f;
    public int witherAmount = 0;
    

    float gravScale;

    void Start()
    {
        gravScale = GetComponent<Rigidbody2D>().gravityScale;

        healthCont = GetComponent<JHealthController>();
        if (ai) {
            enemyAI = GetComponent<JEnemyAI>();
            if (enemyAI == null) {
                enemyFlyingAI = GetComponent<JEnemyAIFlying>();
                normalSpeed = enemyFlyingAI.speed;
            }
            else {
                normalSpeed = enemyAI.speed;
                //ApplyStun(5f);
            }
        }
        else {
            playerCont = GetComponent<JPlayerController>();
            normalSpeed = playerCont.speed;
            //ApplyInvulnerability(5f);
        }

        slowFactors = new List<SpeedMod>();
        dotFactors = new List<DotMod>();
        witherTimer = 0f;
    }

    void Update()
    {
        foreach (SpeedMod speedMod in slowFactors) {
            speedMod.duration -= Time.deltaTime;
        }
        foreach (DotMod dotMod in dotFactors) {
            dotMod.duration -= Time.deltaTime;
        }

        float currentSpeed = CalculateSpeed();
        //print(currentSpeed);

        if (overrideS) {
            currentSpeed = overrideSpeed;
        }

        if (ai) {
            if (enemyAI != null) {
                enemyAI.speed = currentSpeed;
            }
            else {
                enemyFlyingAI.speed = currentSpeed;
            }
        }
        else {
            playerCont.speed = currentSpeed;
        }

        // DOT
        if (dotTimer > 0f) {
            dotTimer -= Time.deltaTime;
        }
        else {
            dotTimer = 1f;
            int dotDamage = CalculateDotDamage();
            if (dotDamage > 0) {
                healthCont.TakeDamage(CalculateDotDamage(), JHealthController.DAMAGETYPE.Dot);
            }
        }

        // WITHER
        if (witherTimer > 0f) {
            witherTimer -= Time.deltaTime;
        }
        else {
            if (witherAmount != 0) {
                healthCont.armor += witherAmount;
                witherAmount = 0;
            }
        }

    }

    public float CalculateSpeed() {
        float speed = normalSpeed;
        for (int i = 0; i < slowFactors.Count; i++) {
            if (slowFactors[i].duration > 0f) {
                speed *= (1f - slowFactors[i].factor);
            }
            else {
                slowFactors.Remove(slowFactors[i]);
                i--;
            }
        }
        return speed;
    }

    public int CalculateDotDamage() {
        int damage = 0;
        for (int i = 0; i < dotFactors.Count; i++) {
            if (dotFactors[i].duration > 0f) {
                if (healthCont.hp > 1 || dotFactors[i].lethal)
                    damage += dotFactors[i].damage;
            }
            else {
                dotFactors.Remove(dotFactors[i]);
                i--;
            }
        }
        return damage;
    }

    public void ApplySlow(float duration, float amount) {
        slowFactors.Add(new SpeedMod(amount, duration));
    }

    public void ApplyStun(float duration) {
        if (!ai) {
            StartCoroutine(playerCont.StunDuration(duration));
            GameObject particles = Instantiate(stunParticles, transform, false);
            particles.transform.localPosition = new Vector3(0f, 0.178f, 0f);
            Destroy(particles, duration);
        }
        else {
            StopCoroutine("StunDuration");
            StartCoroutine(StunDuration(duration));
            GameObject particles = Instantiate(stunParticles, transform, false);
            particles.transform.localPosition = new Vector3(0f, 0.178f, 0f);
            Destroy(particles, duration);
        }
    }

    IEnumerator StunDuration(float duration) {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        stunned = true;

        rb2d.gravityScale = 80f;
        rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(duration);
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb2d.gravityScale = gravScale;
        stunned = false;
    }

    public void ApplyInvulnerability(float duration) {
        GameObject particles = Instantiate(invulnerabilityParticles, transform, false);
        particles.transform.localScale = new Vector3(0.17f, 0.17f, 0f);
        Destroy(particles, duration);
        StartCoroutine(InvulnerabilityDuration(duration));
    }

    IEnumerator InvulnerabilityDuration(float duration) {
        healthCont.invulnerable = true;
        yield return new WaitForSeconds(duration);
        healthCont.invulnerable = false;
    }

    public void ApplyPoison(int id, float duration, int damage, bool lethal) {
        foreach (DotMod dot in dotFactors) {
            if (dot.id == id) {
                if (dot.duration < duration)
                    dot.duration = duration;
                return;
            }
        }
        dotFactors.Add(new DotMod(id, duration, damage, lethal));
    }

    public class DotMod {
        public int id;
        public float duration;
        public int damage;
        public bool lethal;

        public DotMod(int _id, float _duration, int _dmg, bool _lethal) {
            id = _id;
            duration = _duration;
            damage = _dmg;
            lethal = _lethal;
        }
    }

    public class SpeedMod {
        public float factor;
        public float duration;

        public SpeedMod(float _factor, float _duration) {
            factor = _factor;
            duration = _duration;
        }
    }

}
