using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WChainsaw : MonoBehaviour, IItem
{
    public int damagePerSec = 10;
    public int costPerSec = 5;
    private float cooldown = 0.1f;
    private float timer;
    private int fireCount = 0;

    private Animator anim;
    private AudioSource audio;
    private JPlayerController playerCont;
    private bool running = false;

    private List<GameObject> collidingObjects;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        audio = GetComponent<AudioSource>();
        collidingObjects = new List<GameObject>();
        running = false;
        timer = 0f;
    }

    void Update()
    {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (timer > 0f) {
            timer -= Time.deltaTime;
        }

        if (Input.GetKey(Keys.useItem)) {
            Use();
            running = true;
            if (!audio.isPlaying) {
                audio.Play();
            }
            //GetComponent<AudioSource>().Play();
            anim.SetBool("IsRunning", true);
        }
        else {
            running = false;
            if (audio.isPlaying) {
                audio.Stop();
            }
            anim.SetBool("IsRunning", false);
        }
    }

    public void Use() {
        if (timer <= 0f) {
            if (GameMaster.instance.playerEnergy.UseEnergy(costPerSec / 5) || fireCount == 0) {
                fireCount++;
                if (fireCount == 2) {
                    fireCount = 0;
                }
                timer = cooldown;
                int dir = 0;
                if (playerCont.facingRight) {
                    dir = 1;
                }
                else {
                    dir = -1;
                }
                foreach (GameObject target in collidingObjects) {
                    //print((int)(damagePerSec / 10.0));
                    target.GetComponent<JHealthController>().TakeDamage((int)(damagePerSec / 10.0));
                    if (target.GetComponent<Rigidbody2D>() != null) {
                        target.GetComponent<Rigidbody2D>().AddForce(new Vector3(3f * dir, 0f, 0f), ForceMode2D.Impulse);
                    }
                }

                GameMaster.instance.inventory.UseDurability(1);


            }
            else {
                print("Insufficient energy");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag != "Player") {
            if (collision.GetComponent<JHealthController>() != null) {
                if (!collidingObjects.Contains(collision.gameObject)) {
                    collidingObjects.Add(collision.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag != "Player") {
            if (collidingObjects.Contains(collision.gameObject)) {
                collidingObjects.Remove(collision.gameObject);
            }
        }
    }

    public float GetCooldown() {
        return 0f;
    }
}
