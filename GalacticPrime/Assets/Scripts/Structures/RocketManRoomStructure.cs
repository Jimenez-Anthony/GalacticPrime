using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RocketManRoomStructure : MonoBehaviour, IStructureGenerator {
    public new bool enabled = false;
    public float chance = 0.5f;
    public Tilemap structure;
    public Vector2Int maxCoords;
    public Tilemap tilemap;
    public GameObject rocketMan;
    public GameObject background;
    //public ItemStack loot;
    //public DroppedItem droppedItem;
    private System.Random rng;

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
        int emptyLeft = 0;
        int emptyRight = 0;
        int emptyTopLeft = 0;
        int emptyTopRight = 0;

        bool leftSide = false;
        if (FindEmptierSide(startX) == 1) {
            leftSide = true;
        }

        for (int x = 0; x <= maxCoords.x; x++) {
            for (int y = 0; y <= maxCoords.y; y++) {
                //print("Structure bounds: " + structure.cellBounds.xMax + " " + structure.cellBounds.yMax);
                tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), structure.GetTile(new Vector3Int(x, y, 0)));
                //if (y == maxCoords.y) {
                //    if ((x == 1 || x == maxCoords.x - 1)&& tilemap.GetTile(new Vector3Int(startX + x, startY + y + 1, 0)) != GameMaster.instance.levelGenerator.tile) {
                //        tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), null);
                //    }
                //}

                if (y >= 1 && y <= 7) {
                    if (leftSide && x == maxCoords.x) {
                        tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), null);
                        int xOffset = 1;
                        while ((xOffset <= 100 && tilemap.GetTile(new Vector3Int(startX + x + xOffset, startY + y, 0)) != null) || xOffset <= 10) {
                            //print("cleared path");
                            tilemap.SetTile(new Vector3Int(startX + x + xOffset, startY + y, 0), null);
                            GameMaster.instance.levelGenerator.map[startX + x + xOffset, startY + y] = 0;
                            xOffset++;
                        }
                    }
                    else if (!leftSide && x == 0) {
                        tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), null);
                        int xOffset = 1;
                        while ((xOffset <= 100 && tilemap.GetTile(new Vector3Int(startX + x - xOffset, startY + y, 0)) != null) || xOffset <= 10) {
                            //print("cleared path");
                            tilemap.SetTile(new Vector3Int(startX + x - xOffset, startY + y, 0), null);
                            GameMaster.instance.levelGenerator.map[startX + x - xOffset, startY + y] = 0;
                            xOffset++;
                        }
                    }
                }


                //if (y >= 1 && y <= 3) {
                //    if (x == 0 && tilemap.GetTile(new Vector3Int(startX + x - 1, startY + y, 0)) != GameMaster.instance.levelGenerator.tile) {
                //        emptyLeft++;

                //    }
                //    if (x == maxCoords.x && tilemap.GetTile(new Vector3Int(startX + x + 1, startY + y, 0)) != GameMaster.instance.levelGenerator.tile) {
                //        emptyRight++;
                //    }
                //}

                //if (x == 1 && y == maxCoords.y) {
                //    for (int i = 1; i <= 3; i++) {
                //        if (tilemap.GetTile(new Vector3Int(startX + x, startY + y + i, 0)) != GameMaster.instance.levelGenerator.tile) {
                //            emptyTopLeft++;
                //        }
                //    }
                //}
                //if (x == maxCoords.x - 1 && y == maxCoords.y) {
                //    for (int i = 1; i <= 3; i++) {
                //        if (tilemap.GetTile(new Vector3Int(startX + x, startY + y + i, 0)) != GameMaster.instance.levelGenerator.tile) {
                //            emptyTopRight++;
                //        }
                //    }
                //}
                ////print(startX + x + " " + (startY + y));
                structureMap[startX + x, startY + y] = 1;
            }
        }

        Vector3 centerPos = tilemap.CellToWorld(new Vector3Int(startX + 5, startY + 1, 0));
        centerPos = new Vector3(centerPos.x + 0.5f, centerPos.y + 0.5f, 0f);
        //GameObject home = Instantiate(skeletonHome, centerPos, Quaternion.identity);
        //SkeletonKingBehavior boss = Instantiate(skeletonKing, new Vector3(centerPos.x, centerPos.y + 3f, 0f), Quaternion.identity) as SkeletonKingBehavior;
        //boss.home = home.transform;
        Instantiate(rocketMan, new Vector3(centerPos.x, centerPos.y + 3f, 0f), Quaternion.identity);

        //DroppedItem lootClone = Instantiate(droppedItem, centerPos, Quaternion.identity) as DroppedItem;
        //lootClone.itemStack = loot;
        //boss.

        GameObject backgroundClone = Instantiate(background, new Vector3(startX + 0.5f, startY + 0.5f, 0), Quaternion.identity);
        backgroundClone.GetComponent<SpriteRenderer>().size = new Vector2(maxCoords.x + 0.5f, maxCoords.y + 0.5f);

        //if (emptyLeft >= 3) {
        //    tilemap.SetTile(new Vector3Int(startX, startY + 1, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX, startY + 2, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX, startY + 3, 0), null);
        //}
        //if (emptyRight >= 3) {
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 1, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 2, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 3, 0), null);
        //}

        //bool destroyLeft = false;
        //bool destroyRight = false;

        //if (emptyTopLeft >= 3 && emptyTopRight >= 3) {
        //    if (rng.NextDouble() < 0.5) {
        //        destroyLeft = true;
        //    }
        //    else {
        //        destroyRight = true;
        //    }
        //}
        //else if (emptyTopLeft >= 3) {
        //    destroyLeft = true;
        //}
        //else if (emptyTopRight >= 3) {
        //    destroyRight = true;
        //}

        //if (destroyLeft) {
        //    tilemap.SetTile(new Vector3Int(startX + 1, startY + maxCoords.y, 0), null);
        //    for (int i = 1; i <= 4; i++) {
        //        tilemap.SetTile(new Vector3Int(startX + maxCoords.x - 1, startY + i, 0), null);
        //    }
        //}
        //if (destroyRight) {
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x - 1, startY + maxCoords.y, 0), null);
        //    for (int i = 1; i <= 4; i++) {
        //        tilemap.SetTile(new Vector3Int(startX + +1, startY + i, 0), null);
        //    }
        //}
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

    bool WithinMapRange(int x, int y) {
        return x >= 0 && x < structureMap.GetLength(0) && y >= 0 && y < structureMap.GetLength(1);
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
        if (x > 10 && x < (structureMap.GetLength(0) - 10 - maxCoords.x)) {
            return false;
        }

        bool open = false;
        int emptyTiles = 0;

        for (int i = 1; i <= 3; i++) {
            if (tilemap.GetTile(new Vector3Int(x - 1, y + i, 0)) == null) {
                emptyTiles++;
            }
        }
        if (emptyTiles >= 3) {
            open = true;
        }
        emptyTiles = 0;

        for (int i = 1; i <= 3; i++) {
            if (tilemap.GetTile(new Vector3Int(x + maxCoords.x + 1, y + i, 0)) == null) {
                emptyTiles++;
            }
        }
        if (emptyTiles >= 3) {
            open = true;
        }
        emptyTiles = 0;

        for (int i = 1; i <= 3; i++) {
            if (tilemap.GetTile(new Vector3Int(x + 1, y + maxCoords.y + i, 0)) == null) {
                emptyTiles++;
            }
        }
        if (emptyTiles >= 3) {
            open = true;
        }
        emptyTiles = 0;

        for (int i = 1; i <= 3; i++) {
            if (tilemap.GetTile(new Vector3Int(x + maxCoords.x - 1, y + maxCoords.y + i, 0)) == null) {
                emptyTiles++;
            }
        }
        if (emptyTiles >= 3) {
            open = true;
        }
        emptyTiles = 0;

        if (open)
            return true;

        return true;

    }
}
