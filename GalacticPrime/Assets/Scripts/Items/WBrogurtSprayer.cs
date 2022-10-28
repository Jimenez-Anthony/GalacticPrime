using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBrogurtSprayer : MonoBehaviour, IItem
{
    public int damage;
    public float fireRate;
    private float fireCooldown;
    private float fireTimer;
    private bool shooting;

    private float useEnergyCooldown = 0.33f;
    private float useEnergyTimer;

    public BrogurtParticle brogurt;

    public Transform firePoint;
    private JPlayerController playerCont;
    private AudioSource audio;
    private int dir = 0;

    void Start()
    {
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        audio = GetComponent<AudioSource>();
        firePoint = transform.GetChild(0);
        fireCooldown = 1f / fireRate;
        fireTimer = fireCooldown;
        useEnergyTimer = 0f;
    }

    void Update()
    {
        if (fireTimer > 0f) {
            fireTimer -= Time.deltaTime;
        }
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (useEnergyTimer > 0f && shooting) {
            useEnergyTimer -= Time.deltaTime;
        }
        else if (shooting) {
            useEnergyTimer = useEnergyCooldown;
            GameMaster.instance.playerEnergy.UseEnergy(1);
        }

        if (playerCont.facingRight) {
            dir = 1;
        }
        else {
            dir = -1;
        }

        if (Input.GetKey(Keys.useItem)) {
            shooting = true;
            Use();
        }
        else {
            shooting = false;
            audio.Stop();
        }
    }

    public float GetCooldown() {
        return fireTimer / fireCooldown;
    }

    public void Use() {
        if (fireTimer <= 0f) {
            if (GameMaster.instance.playerEnergy.energy > 0) {
                if (!audio.isPlaying) {
                    audio.Play();
                }
                fireTimer = fireCooldown;
                Vector3 firePos = new Vector3(firePoint.position.x + Random.Range(-0.2f, 0.2f), firePoint.position.y + Random.Range(-0.2f, 0.2f), 0f);
                BrogurtParticle clone = Instantiate(brogurt, firePos, Quaternion.identity);
                Vector3 force = new Vector3(dir * 20f + Random.Range(-5f, 5f), 5f + Random.Range(-2f, 2f), 0f);
                clone.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

                GameMaster.instance.inventory.UseDurability(1);

            }
            else {
                print("Insufficient energy");
            }
        }
    }
}
