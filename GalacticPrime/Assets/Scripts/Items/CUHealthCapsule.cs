using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUHealthCapsule : MonoBehaviour, IItemPassive {

    public float GetCooldown() {
        return 0f;
    }

    public void OnEquip() {
        GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(5);
    }

    public void OnUnequip() {
        GameMaster.instance.player.GetComponent<JHealthController>().IncreaseMaxHealth(-5);
    }

    public void Use() {

    }
}
