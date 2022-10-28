using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StalkerRoomStructure : MonoBehaviour, IStructureGenerator {

    public new bool enabled = false;
    public float chance = 0.5f;
    public Tilemap structure;
    public Vector2Int maxCoords;
    public Tilemap tilemap;
    //public GameObject skeletonHome;
    public GameObject background;
    //public ItemStack loot;
    //public DroppedItem droppedItem;
    private System.Random rng;
    public GameObject stalker;

    private int[,] structureMap;

    void Start() {
        tilemap = GameMaster.instance.tilemap;

    }

    void Update() {

    }

    public void Generate(int[,] structureMap) {
        if (!enabled) {
            return;
        }

        //print("Generate");

        tilemap = GameMaster.instance.tilemap;
        rng = GameMaster.instance.levelGenerator.rng;

        this.structureMap = structureMap;

        for (int x = 1; x < structureMap.GetLength(0) - 1; x++) {
            for (int y = 1; y < structureMap.GetLength(1) - 1; y++) {
                if (GameMaster.instance.levelGenerator.bossCount > 0) {
                    return;
                }

                if (rng.NextDouble() * 100 < chance / 10f) {
                    //print("Testing for generation");
                    if (CheckSpawnCondition(x, y)) {
                        //print("Generating");
                        GenerateAt(x, y);
                        GameMaster.instance.levelGenerator.bossCount++;
                    }
                }
            }
        }
    }

    void GenerateAt(int startX, int startY) {

        bool leftSide = false;
        if (FindEmptierSide(startX) == 1) {
            leftSide = true;
        }

        GameObject backgroundClone = Instantiate(background, new Vector3(startX, startY, 0), Quaternion.identity);
        backgroundClone.GetComponent<SpriteRenderer>().size = new Vector2(maxCoords.x + 1, maxCoords.y + 1);
        Instantiate(stalker, new Vector3(startX + 2.5f, startY + 2.5f, 0), Quaternion.identity);

        for (int x = 0; x <= maxCoords.x; x++) {
            for (int y = 0; y <= maxCoords.y; y++) {
                tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), structure.GetTile(new Vector3Int(x, y, 0)));
                structureMap[startX + x, startY + y] = 1;

                if (y >= 1 && y <= 3) {
                    if (leftSide && x == maxCoords.x) {
                        tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), null);
                        int xOffset = 1;
                        while (xOffset <= 100 && tilemap.GetTile(new Vector3Int(startX + x + xOffset, startY + y, 0)) != null) {
                            print("cleared path left");
                            tilemap.SetTile(new Vector3Int(startX + x + xOffset, startY + y, 0), null);
                            GameMaster.instance.levelGenerator.map[startX + x + xOffset, startY + y] = 0;
                            xOffset++;
                        }
                    }
                    else if (!leftSide && x == 0) {
                        tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), null);
                        int xOffset = 1;
                        while (xOffset <= 100 && tilemap.GetTile(new Vector3Int(startX + x - xOffset, startY + y, 0)) != null) {
                            print("cleared path right");
                            tilemap.SetTile(new Vector3Int(startX + x - xOffset, startY + y, 0), null);
                            GameMaster.instance.levelGenerator.map[startX + x - xOffset, startY + y] = 0;
                            xOffset++;
                        }
                    }
                }
            }
        }
    }

    int FindEmptierSide(int startX) {
        int leftCount = 0;
        for (int x = 0; x < startX; x++) {
            for (int y = 0; y < structureMap.GetLength(1); y++) {
                if (GameMaster.instance.levelGenerator.map[x, y] == 0) {
                    leftCount++;
                }
            }
        }
        int rightCount = 0;
        for (int x = startX + 1; x < structureMap.GetLength(0); x++) {
            for (int y = 0; y < structureMap.GetLength(1); y++) {
                if (GameMaster.instance.levelGenerator.map[x, y] == 0) {
                    rightCount++;
                }
            }
        }
        if (leftCount > rightCount) {
            return -1;
        }
        return 1;
    }

    bool CheckSpawnCondition(int x, int y) {
        if (nearOtherStructures(x, y, 10)) {
            return false;
        }
        if (!WithinMapRange(x + maxCoords.x, y + maxCoords.y)) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null) {
            return false;
        }
        for (int i = 0; i <= maxCoords.x; i++) {
            if (tilemap.GetTile(new Vector3Int(x + i, y - 1, 0)) == null) {
                return false;
            }
            if (tilemap.GetTile(new Vector3Int(x + i, y + maxCoords.y + 1, 0)) == null) {
                //return false;
            }
        }
        return true;
    }

    bool WithinMapRange(int x, int y) {
        return x >= 0 && x < structureMap.GetLength(0) && y >= 0 && y < structureMap.GetLength(1);
    }

    bool nearOtherStructures(int checkX, int checkY, int radius) {
        for (int x = 0; x < structureMap.GetLength(0); x++) {
            for (int y = 0; y < structureMap.GetLength(1); y++) {
                if (structureMap[x, y] == 1) {
                    if (Mathf.Abs(checkX - x) < radius && Mathf.Abs(checkY - y) < radius) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
