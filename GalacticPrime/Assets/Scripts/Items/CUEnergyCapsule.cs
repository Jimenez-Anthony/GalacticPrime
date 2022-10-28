using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUEnergyCapsule : MonoBehaviour, IItemPassive {

    public float GetCooldown() {
        return 0f;
    }

    public void OnEquip() {
        GameMaster.instance.player.GetComponent<JEnergyController>().IncreaseEnergyRegen(1);
    }

    public void OnUnequip() {
        GameMaster.instance.player.GetComponent<JEnergyController>().IncreaseEnergyRegen(-1);
    }

    public void Use() {
    }
}
