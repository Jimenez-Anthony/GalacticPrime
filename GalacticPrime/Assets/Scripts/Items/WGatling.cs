using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGatling : MonoBehaviour
{
    public int damage;
    public float fireRate;
    public int energyCost = 1;

    private float fireCooldown;
    private float fireTimer;

    public JBullet bullet;
    public Transform firePoint;
    public string fireSound;

    private JPlayerController playerCont;
    private SpriteRenderer sprite;

    public Sprite idleSprite;
    public Sprite fireSprite;

    void Start() {
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        sprite = GetComponent<SpriteRenderer>();
        fireCooldown = 1f / fireRate;
        fireTimer = fireCooldown;
    }

    void OnEnable() {
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update() {
        if (fireTimer > 0f) {
            fireTimer -= Time.deltaTime;
        }
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (Input.GetKey(Keys.useItem)) {
            sprite.sprite = fireSprite;
            Use();
        }
        else {
            sprite.sprite = idleSprite;
        }
    }

    public void Use() {
        if (fireTimer <= 0f) {
            if (GameMaster.instance.playerEnergy.UseEnergy(energyCost)) {
                fireTimer = fireCooldown;
                JBullet clone = Instantiate(bullet, firePoint.transform.position, Quaternion.identity) as JBullet;
                clone.damage = damage;
                clone.speed = 20f;
                Destroy(clone.gameObject, 2f);
                if (playerCont.facingRight) {
                    clone.faceDir = 1;
                }
                else {
                    clone.faceDir = -1;
                }

                GameMaster.instance.inventory.UseDurability(1);

            }
            else {
                print("Insufficient energy");
            }
        }
    }

    public float GetCooldown() {
        return fireTimer / fireCooldown;
    }
}
