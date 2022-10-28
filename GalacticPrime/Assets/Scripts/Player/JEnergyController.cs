using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JEnergyController : MonoBehaviour
{

    public int maxEnergy = 50;
    public int energy;

    public int energyRegenAmount;
    private float regenTimer;

    // Start is called before the first frame update
    void Start()
    {
        energy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        if (regenTimer > 0) {
            regenTimer -= Time.deltaTime;
        }
        else {
            regenTimer = 1f;
            AddEnergy(energyRegenAmount);
        }
    }

    public void AddEnergy(int amount) {
        energy += amount;
        if (energy > maxEnergy) {
            energy = maxEnergy;
        }
    }

    public bool UseEnergy(int amount) {
        if (energy < amount) {
            return false;
        }
        energy -= amount;
        return true;
    }

    public void IncreaseMaxEnergy(int amount) {
        int newMax = maxEnergy + amount;
        float fullPercent = (float)energy / maxEnergy;
        maxEnergy = newMax;
        energy = (int)(fullPercent * maxEnergy + 0.5f);
    }

    public void IncreaseEnergyRegen(int amount) {
        energyRegenAmount += amount;
    }
}
