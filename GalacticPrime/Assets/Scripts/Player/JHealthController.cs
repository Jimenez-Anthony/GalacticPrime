using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHealthController : MonoBehaviour
{
    public int maxHP;
    public string hurtSound;
    public int hp;
    public int armor;
    public bool invulnerable = false;
    public bool environmentalImmune = false;

    public int naturalRegenAmount = 0;
    private float regenTimer;

    private IDeathController deathCont;
    private IGetsHurt hurtCont;

    public enum DAMAGETYPE {Bullet, Melee, Dot, Environmental, Enemy}

    void Start()
    {
        hp = maxHP;
        //armor = 0;
        invulnerable = false;
        regenTimer = 1f;
        deathCont = GetComponent<IDeathController>();
        hurtCont = GetComponent<IGetsHurt>();

        if (deathCont == null) {
            deathCont = transform.parent.GetComponent<IDeathController>();
        }
        if (hurtCont == null) {
            hurtCont = transform.parent.GetComponent<IGetsHurt>();
        }
    }

    public void TakeDamage(int dmg, DAMAGETYPE type = DAMAGETYPE.Bullet) {
        if (invulnerable) {
            return;
        }
        if (environmentalImmune && (type == DAMAGETYPE.Environmental || type == DAMAGETYPE.Dot)) {
            return;
        }

        if (type != DAMAGETYPE.Dot) {
            dmg -= armor;
            if (dmg < 0) {
                dmg = 0;
            }
        }

        //print(dmg + " " + gameObject.tag);

        hp -= dmg;

        if (GameMaster.instance.player == gameObject) {
            GameMaster.instance.gameStats.damageTaken += dmg;
        }
        else {
            if (type != DAMAGETYPE.Enemy && type != DAMAGETYPE.Environmental) {
                GameMaster.instance.gameStats.damageDealt += dmg;
            }
        }

        if (hurtSound != "") {
            AudioManager.instance.Play(hurtSound);
        }

        if (hp <= 0) {
            Die();
        }
        else if (hurtCont != null) {
            hurtCont.OnHurt();
        }
    }

    public bool Heal(int amount, bool overHeal = false) {
        if (hp >= maxHP && !overHeal) {
            return false;
        }
        else {
            hp += amount;

            if (GameMaster.instance.player == gameObject) {
                if (hp > maxHP && !overHeal) {
                    GameMaster.instance.gameStats.amountHealed += hp + amount - maxHP;
                }
                else {
                    GameMaster.instance.gameStats.amountHealed += amount;
                }
            }

            if (hp > maxHP && !overHeal) {
                hp = maxHP;
            }
            return true;
        }
    }

    public void IncreaseMaxHealth(int amount) {
        int newMax = maxHP + amount;
        float fullPercent = (float)hp / maxHP;
        maxHP = newMax;
        hp = (int)(fullPercent * maxHP + 0.5f);
    }

    public void IncreaseArmor(int amount) {
        armor += amount;
    }

    public void Die() {
        deathCont.OnDeath();
    }

    void Update() {
        if (transform.position.y < -100) {
            TakeDamage(999);
        }

        if (hp <= 0) {
            Die();
        }

        if (regenTimer > 0f) {
            regenTimer -= Time.deltaTime;
        }
        else {
            if (naturalRegenAmount != 0) {
                regenTimer = 2f;
                if (hp < maxHP) {
                    Heal(naturalRegenAmount);
                }
            }
        }

    }
}
