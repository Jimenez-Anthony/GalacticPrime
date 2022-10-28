using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LootRoomStructure : MonoBehaviour, IStructureGenerator
{

    public new bool enabled = false;
    public float chance = 0.5f;
    public Tilemap structure;
    public Vector2Int maxCoords;
    public Tilemap tilemap;
    private System.Random rng;
    public GameObject background;

    private int[,] structureMap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameMaster.instance.tilemap;

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
                if (rng.NextDouble() * 100 < chance / 10f) {
                    //print("Testing for generation");
                    if (CheckSpawnCondition(x,y)) {
                        //print("Generating");
                        GenerateAt(x, y);
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

        for (int x = 0; x <= maxCoords.x; x++) {
            for (int y = 0; y <= maxCoords.y; y++) {
                //print("Structure bounds: " + structure.cellBounds.xMax + " " + structure.cellBounds.yMax);
                tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), structure.GetTile(new Vector3Int(x, y, 0)));
                //if (y == maxCoords.y) {
                //    if ((x == 1 || x == maxCoords.x - 1)&& tilemap.GetTile(new Vector3Int(startX + x, startY + y + 1, 0)) != GameMaster.instance.levelGenerator.tile) {
                //        tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), null);
                //    }
                //}

                if (x == 0 && y > 0 && y < maxCoords.y && tilemap.GetTile(new Vector3Int(startX + x - 1, startY + y, 0)) != GameMaster.instance.levelGenerator.tile) {
                    tilemap.SetTile(new Vector3Int(startX, startY + y, 0), null);
                }
                if (x == maxCoords.x && y > 0 && y < maxCoords.y && tilemap.GetTile(new Vector3Int(startX + x + 1, startY + y, 0)) != GameMaster.instance.levelGenerator.tile) {
                    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + y, 0), null);
                }

                if (y >= 1 && y <= 4) {
                    if (x == 0 && tilemap.GetTile(new Vector3Int(startX + x - 1, startY + y, 0)) != GameMaster.instance.levelGenerator.tile) {
                        emptyLeft++;

                    }
                    if (x == maxCoords.x && tilemap.GetTile(new Vector3Int(startX + x + 1, startY + y, 0)) != GameMaster.instance.levelGenerator.tile) {
                        emptyRight++;
                    }
                }

                if (x == 1 && y == maxCoords.y) {
                    for (int i = 1; i <= 3; i++) {
                        if (tilemap.GetTile(new Vector3Int(startX + x, startY + y + i, 0)) != GameMaster.instance.levelGenerator.tile) {
                            emptyTopLeft++;
                        }
                    }
                }
                if (x == maxCoords.x - 1 && y == maxCoords.y) {
                    for (int i = 1; i <= 3; i++) {
                        if (tilemap.GetTile(new Vector3Int(startX + x, startY + y + i, 0)) != GameMaster.instance.levelGenerator.tile) {
                            emptyTopRight++;
                        }
                    }
                }
                //print(startX + x + " " + (startY + y));
                structureMap[startX + x, startY + y] = 1;
            }
        }

        //if (emptyLeft >= 3) {
        //    tilemap.SetTile(new Vector3Int(startX, startY + 1, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX, startY + 2, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX, startY + 3, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX, startY + 4, 0), null);
        //}
        //if (emptyRight >= 3) {
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 1, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 2, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 3, 0), null);
        //    tilemap.SetTile(new Vector3Int(startX + maxCoords.x, startY + 4, 0), null);
        //}

        bool destroyLeft = true;
        bool destroyRight = true;

        if (emptyTopLeft >= 3 && emptyTopRight >= 3) {
            if (rng.NextDouble() < 0.5) {
                destroyLeft = true;
            }
            else {
                destroyRight = true;
            }
        }
        else if (emptyTopLeft >= 3) {
            destroyLeft = true;
        }
        else if (emptyTopRight >= 3) {
            destroyRight = true;
        }

        if (destroyLeft) {
            tilemap.SetTile(new Vector3Int(startX + 1, startY + maxCoords.y, 0), null);
            structureMap[startX + 1, startY + maxCoords.y] = 0;
            GameMaster.instance.levelGenerator.map[startX + 1, startY + maxCoords.y] = 0;
            for (int i = 1; i <= 4; i++) {
                //tilemap.SetTile(new Vector3Int(startX + maxCoords.x - 1, startY + i, 0), null);
            }
        }
        if (destroyRight) {
            tilemap.SetTile(new Vector3Int(startX + maxCoords.x - 1, startY + maxCoords.y, 0), null);
            structureMap[startX + maxCoords.x - 1, startY + maxCoords.y] = 0;
            GameMaster.instance.levelGenerator.map[startX + maxCoords.x - 1, startY + maxCoords.y] = 0;
            for (int i = 1; i <= 4; i++) {
                //tilemap.SetTile(new Vector3Int(startX + + 1, startY + i, 0), null);
            }
        }

        GameObject backgroundClone = Instantiate(background, new Vector3(startX + 0.5f, startY + 0.5f, 0), Quaternion.identity);
        backgroundClone.GetComponent<SpriteRenderer>().size = new Vector2(maxCoords.x + 0.5f, maxCoords.y);
    }

    bool nearOtherStructures(int checkX, int checkY, int radius) {
        for (int x = 0; x < structureMap.GetLength(0); x++) {
            for (int y = 0; y < structureMap.GetLength(1); y++) {
                if (structureMap[x,y] == 1) {
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
        if (!WithinMapRange(x + structure.cellBounds.xMax, y + structure.cellBounds.yMax)) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null) {
            return false;
        }
        for (int i = 0; i <= maxCoords.x; i++) {
            if (tilemap.GetTile(new Vector3Int(x + i, y - 1, 0)) == null) {
                return false;
            }
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

        return false;

    }

}
