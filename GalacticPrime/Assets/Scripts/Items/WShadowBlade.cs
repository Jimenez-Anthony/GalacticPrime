using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WShadowBlade : MonoBehaviour, IItem
{

    public float cooldown = 5f;
    private float timer;
    public float comboCooldown = 0.5f;
    private float comboTimer;
    public int damage = 10;
    public int energyCost = 10;

    public ShadowBall shadowBall;
    public GameObject shadowParticles;
    private ShadowBall ballClone;
    private Vector3 lastLoc;
    public float tpDistance = 2f;

    private int dir;
    public bool comboStarted;


    void Start()
    {
        timer = 0f;
        comboTimer = comboCooldown;
    }

    void Update()
    {
        if (timer > 0f) {
            timer -= Time.deltaTime;
        }

        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (comboStarted) {
            comboTimer -= Time.deltaTime;
        }
        if (comboStarted && comboTimer <= 0f) {
            comboStarted = false;
            comboTimer = comboCooldown;
            timer = cooldown;
        }


        dir = 0;
        if (GameMaster.instance.player.transform.position.x - transform.position.x > 0) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        if (ballClone != null) {
            GameMaster.instance.player.transform.position = ballClone.transform.position;
        }

        if (timer <= 0f && Input.GetKey(Keys.useItem) && ballClone == null) {
            comboStarted = true;
            comboTimer = comboCooldown;
            if (comboTimer > 0f) {
                if (Input.GetKey(Keys.left) && Input.GetKey(Keys.right)) {
                    FireBall(0, 0);
                }
                else if (Input.GetKeyDown(Keys.ladderUp) && Input.GetKeyDown(Keys.ladderDown)) {
                    FireBall(0, 0);
                }
                else if (Input.GetKeyDown(Keys.left) && Input.GetKeyDown(Keys.ladderUp)) {
                    FireBall(-1, 1);
                }
                else if (Input.GetKeyDown(Keys.left) && Input.GetKeyDown(Keys.ladderDown)) {
                    FireBall(-1, -1);
                }
                else if (Input.GetKeyDown(Keys.right) && Input.GetKeyDown(Keys.ladderUp)) {
                    FireBall(1, 1);
                }
                else if (Input.GetKeyDown(Keys.right) && Input.GetKeyDown(Keys.ladderDown)) {
                    FireBall(1, -1);
                }
                else if (Input.GetKeyDown(Keys.left)) {
                    FireBall(-1, 0);
                }
                else if (Input.GetKeyDown(Keys.right)) {
                    FireBall(1, 0);
                }
                else if (Input.GetKeyDown(Keys.ladderUp)) {
                    FireBall(0, 1);
                }
                else if (Input.GetKeyDown(Keys.ladderDown)) {
                    FireBall(0, -1);
                }
            }
        }
    }

    void FireBall(int x, int y) {
        if (GameMaster.instance.playerEnergy.UseEnergy(energyCost)) {
            GameMaster.instance.player.GetComponent<SpriteRenderer>().enabled = false;
            GameMaster.instance.player.GetComponent<FallDamage>().enabled = false;
            GameMaster.instance.player.GetComponent<JHealthController>().invulnerable = true;
            GetComponent<SpriteRenderer>().enabled = false;
            lastLoc = GameMaster.instance.player.transform.position;
            ballClone = Instantiate(shadowBall, transform.position, Quaternion.identity);
            Vector3 targetLoc = new Vector3(transform.position.x + x * tpDistance, transform.position.y + y * tpDistance, 0f);
            ballClone.gun = this;
            ballClone.damage = damage;
            ballClone.targetPos = targetLoc;
            GameObject particles = Instantiate(shadowParticles, GameMaster.instance.player.transform.position, Quaternion.identity);
            Destroy(particles, 1f);

            GameMaster.instance.inventory.UseDurability(1);

        }
    }

    public void BallLanded(Vector3 loc) {
        comboTimer = comboCooldown;
        GameMaster.instance.player.transform.position = new Vector3(loc.x, loc.y + 0.5f, 0f);
        GameMaster.instance.player.GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(EndShadow());
        GameObject particles = Instantiate(shadowParticles, GameMaster.instance.player.transform.position, Quaternion.identity);
        Destroy(particles, 1f);
    }

    public void GoBack() {
        comboTimer = comboCooldown;
        GameMaster.instance.player.transform.position = lastLoc;
        GameMaster.instance.player.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(EndShadow());
        GameObject particles = Instantiate(shadowParticles, GameMaster.instance.player.transform.position, Quaternion.identity);
        Destroy(particles, 1f);
        GetComponent<SpriteRenderer>().enabled = true;
    }

    IEnumerator EndShadow() {
        yield return new WaitForSeconds(0.25f);
        GameMaster.instance.player.GetComponent<JHealthController>().invulnerable = false;
        yield return new WaitForSeconds(0.25f);
        GameMaster.instance.player.GetComponent<FallDamage>().enabled = true;
    }

    public void Use() {

    }

    void OnDestroy() {
        comboTimer = comboCooldown;
        //GameMaster.instance.player.transform.position = lastLoc;
        GameMaster.instance.player.GetComponent<SpriteRenderer>().enabled = true;
        GameMaster.instance.player.GetComponent<FallDamage>().enabled = true;
        GameMaster.instance.player.GetComponent<JHealthController>().invulnerable = false;
        //GameObject particles = Instantiate(shadowParticles, GameMaster.instance.player.transform.position, Quaternion.identity);
        //Destroy(particles, 1f);
        GetComponent<SpriteRenderer>().enabled = true;
        Destroy(ballClone);
    }

    public float GetCooldown() {
        return timer / cooldown;
    }
}
