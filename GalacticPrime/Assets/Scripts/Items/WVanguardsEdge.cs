using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVanguardsEdge : MonoBehaviour, IItem
{
    public int damage;
    public float fireRate;
    public int energyCost = 1;

    private float fireCooldown;
    private float fireTimer;

    public VanguardGroup vanguardGroup;
    public Transform firePoint;
    public string fireSound;

    private JPlayerController playerCont;

    void Start() {
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        fireCooldown = 1f / fireRate;
        fireTimer = 0f;
    }

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
                //GetComponent<AudioSource>().Play();
                VanguardGroup vanguardClone = Instantiate(vanguardGroup, firePoint.transform.position, Quaternion.identity) as VanguardGroup;
                vanguardClone.facingRight = playerCont.facingRight;
                int dir = -1;
                if (playerCont.facingRight) {
                    dir = 1;
                }
                vanguardClone.targetPos = new Vector3(transform.position.x + dir * 10f, transform.position.y, transform.position.z);
                vanguardClone.damage = damage;

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
