using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBattery : MonoBehaviour, IItem
{

    public int amount = 20;
    public GameObject rechargeParticles;
    private JEnergyController energy;

    // Start is called before the first frame update
    void Start()
    {
        energy = GameMaster.instance.playerEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());
    }

    public void Use() {
        if (energy.energy != energy.maxEnergy) {
            energy.AddEnergy(amount);
            if (GameMaster.instance.inventory.RemoveSelectedItem()) {
                Destroy(this.gameObject);
            }
            GameObject particles = Instantiate(rechargeParticles, GameMaster.instance.player.transform, false);
            particles.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            //particles.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Destroy(particles, 2f);
        }
    }

    public float GetCooldown() {
        return 0f;
    }
}
