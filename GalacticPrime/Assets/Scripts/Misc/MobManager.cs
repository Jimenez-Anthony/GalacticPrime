using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MobManager : MonoBehaviour
{
    public float spawnChance = 5f;
    public MobSpawnTable[] spawns;

    private System.Random rng;
    private Tilemap tilemap;
    //public RuleTile startTile;
    public Vector3Int startPos;

    void Start()
    {
        rng = GameMaster.instance.levelGenerator.rng;
        tilemap = GameMaster.instance.tilemap;
    }

    public void SpawnMobs() {
        int count = 0;
        rng = GameMaster.instance.levelGenerator.rng;
        tilemap = GameMaster.instance.tilemap;

        for (int x = 1; x <= tilemap.cellBounds.xMax - 1; x++) {
            for (int y = 1; y <= tilemap.cellBounds.yMax - 1; y++) {
                if (!NearSpawn(x, y, 5) && OnGround(x, y)) {
                    if (rng.NextDouble() * 100f < spawnChance) {
                        //print("Spawning mob");
                        GameObject spawn = GetSpawn();
                        Vector3 worldPos = tilemap.CellToWorld(new Vector3Int(x, y, 0));
                        worldPos = new Vector3(worldPos.x, worldPos.y + 0.5f, 0f);
                        Instantiate(spawn, worldPos, Quaternion.identity);
                        count++;
                    }
                }
            }
        }
        print("[Mob Manager] Spawned " + count + " Mobs");
    }

    bool NearSpawn(int x, int y, int range) {
        if (Mathf.Abs(x - startPos.x) <= range) {
            return true;
        }
        if (Mathf.Abs(y - startPos.y) <= range) {
            return true;
        }
        return false;
    }

    bool OnGround(int x, int y) {
        if (tilemap.GetTile(new Vector3Int(x, y, 0)) != null) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x, y - 1, 0)) != GameMaster.instance.levelGenerator.tile) {
            return false;
        }
        return true;
    }

    public GameObject GetSpawn() {
        //rng = GameMaster.instance.levelGenerator.rng;

        float totalWeight = 0f;
        foreach (MobSpawnTable spawn in spawns) {
            totalWeight += spawn.weight;
        }

        double num = rng.NextDouble() * totalWeight;
        float checkNum = 0f;
        foreach (MobSpawnTable spawn in spawns) {
            checkNum += spawn.weight;
            if (num <= checkNum) {
                return spawn.mob;
            }
        }

        return null;
    }
}

[System.Serializable]
public class MobSpawnTable {
    public GameObject mob;
    public int weight;
}
