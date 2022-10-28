using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{

    public LootTable[] crate;
    public LootTable[] goldenCrate;

    private System.Random rng;

    // Start is called before the first frame update
    void Start()
    {
        rng = GameMaster.instance.levelGenerator.rng;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ItemStack GetItem(LootTable[] table) {
        rng = GameMaster.instance.levelGenerator.rng;

        float totalWeight = 0f;
        foreach (LootTable loot in table) {
            totalWeight += loot.weight;
        }

        double num = rng.NextDouble() * totalWeight;
        float checkNum = 0f;
        foreach (LootTable loot in table) {
            checkNum += loot.weight;
            if (num <= checkNum) {
                return loot.itemStack;
            }
        }
        return null;
    }
}

[System.Serializable]
public class LootTable {
    public ItemStack itemStack;
    public float weight;
}
