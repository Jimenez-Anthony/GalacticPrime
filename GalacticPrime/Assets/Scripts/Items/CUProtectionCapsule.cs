using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUProtectionCapsule : MonoBehaviour, IItemPassive
{
    public float GetCooldown() {
        return 0f;
    }

    public void OnEquip() {
        GameMaster.instance.player.GetComponent<JHealthController>().IncreaseArmor(2);
    }

    public void OnUnequip() {
        GameMaster.instance.player.GetComponent<JHealthController>().IncreaseArmor(-2);
    }

    public void Use() {

    }
}
