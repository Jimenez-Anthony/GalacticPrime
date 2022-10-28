using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLaserRifle : MonoBehaviour, IItem
{
    public int damage;
    public float fireRate;
    public int energyCost = 1;

    private float fireCooldown;
    private float fireTimer;

    public LaserBeam bullet;
    public Transform firePoint;
    public string fireSound;

    private JPlayerController playerCont;
    private Animator anim;

    void Start() {
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        anim = GetComponent<Animator>();
        fireCooldown = 1f / fireRate;
        fireTimer = fireCooldown;
    }

    // Update is called once per frame
    void Update() {
        if (fireTimer > 0f) {
            fireTimer -= Time.deltaTime;
        }
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

    }

    public void Use() {
        if (fireTimer <= 0f) {
            if (GameMaster.instance.playerEnergy.UseEnergy(energyCost)) {
                fireTimer = fireCooldown;
                StartCoroutine(Fire());

            }
            else {
                print("Insufficient energy");
            }
        }
    }

    IEnumerator Fire() {
        anim.SetTrigger("Fire");
        yield return new WaitForSeconds(0.3f);
        LaserBeam clone = Instantiate(bullet, firePoint.transform.position, Quaternion.identity) as LaserBeam;
        GetComponent<AudioSource>().Play();
        clone.damage = damage;
        Destroy(clone.gameObject, 2f);
        if (playerCont.facingRight) {
            clone.faceDir = 1;
        }
        else {
            clone.faceDir = -1;
        }

        GameMaster.instance.inventory.UseDurability(1);

    }

    public float GetCooldown() {
        return fireTimer / fireCooldown;
    }
}
