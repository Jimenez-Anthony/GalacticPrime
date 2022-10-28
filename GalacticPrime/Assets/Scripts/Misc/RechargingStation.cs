using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargingStation : MonoBehaviour {
    //public GameObject particles;
    public GameObject rechargeParticles;

    public bool activated = false;
    private float stayTimer;

    public float healCooldown = 0.25f;
    private float healTimer;

    public float healDuration = 5f;
    private float activatedTimer;

    float speed = 2.5f;
    float height = 0.01f;

    private Rigidbody2D rb2d;

    void Start() {
        stayTimer = 0f;
        activatedTimer = 0f;
        healTimer = healCooldown;
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (GameMaster.instance.gameState == GameMaster.GAMESTATE.paused) {
            rb2d.velocity = Vector2.zero;
            return;
        }

        if (!activated) {
            Vector3 pos = transform.position;
            float newY = Mathf.Sin(Time.time * speed * Time.timeScale);
            transform.position = new Vector3(pos.x, pos.y + newY * height, pos.z);
        }
        else {
            rb2d.WakeUp();
            activatedTimer += Time.deltaTime;

            if (healTimer > 0f) {
                healTimer -= Time.deltaTime;
            }
            else {
                healTimer = healCooldown;
            }
        }

        if (activatedTimer > healDuration) {
            Vector3Int pos = GameMaster.instance.tilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y - 1.5f, 0f));
            GameMaster.instance.tilemap.SetTile(pos, GameMaster.instance.levelGenerator.tile);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Player") {
            stayTimer += Time.deltaTime;
            if (stayTimer > 1.3f) {
                activated = true;
                if (healTimer <= 0f) {
                    GameMaster.instance.player.GetComponent<JEnergyController>().AddEnergy(3);
                    GameObject particles = Instantiate(rechargeParticles, GameMaster.instance.player.transform, false);
                    particles.transform.localPosition = new Vector3(0f, 0.2f, 0f);
                    Destroy(particles, 0.5f);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player" && !activated) {
            stayTimer = 0f;
        }
    }
}
