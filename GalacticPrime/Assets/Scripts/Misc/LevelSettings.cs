using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//public enum LEVELTYPE {surface, cave};
[CreateAssetMenu(fileName = "New Level Settings", menuName = "LevelSettings")]
public class LevelSettings : ScriptableObject
{
    public int lvlID;
    public JProceduralGenerator.Leveltype lvlType;
    public MobSpawnTable[] mobSpawns;
    public float mobSpawnChance;
    public Vector2Int mapSize;
    public bool generateSpikes;
    public float spikeChance = 5f;
    public bool generateCrates;
    public bool generateLadders = true;
    public bool generateGrass = true;

    public bool generateArrowTrap;
    public float arrowTrapChance = 0.5f;

    public bool generateHealingStation;
    public float healingStationChance = 1f;
    public bool generateRechargingStation;
    public float rechargingStationChance = 1f;

    public bool generateStrcuture;
    public bool spawnSkeletonKing;
    public float skeletonKingChance = 3f;
    public bool spawnMechPortal;
    public float mechPortalChance = 0.75f;
    public bool generateStalker = false;
    public bool generateRocketMan = false;
    public RuleTile baseTile;

    public bool overrideCrateLootable;
    public LootTable[] crateLootTable;

    public bool overrideGoldenCrateLootable;
    public LootTable[] goldenCrateLootTable;
}
