using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHealingItem : MonoBehaviour, IItem {

    public int amount;
    public GameObject healParticles;

    void Update()
    {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

    }

    public void Use() {
        JHealthController health = GameMaster.instance.player.GetComponent<JHealthController>();
        if (health.Heal(amount)) {
            if (GameMaster.instance.inventory.RemoveSelectedItem()) {
                Destroy(this.gameObject);
            }
            GameObject particles = Instantiate(healParticles, GameMaster.instance.player.transform.GetChild(0), false);
            particles.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Destroy(particles, 2f);

        }
    }

    public float GetCooldown() {
        return 0f;
    }
}
