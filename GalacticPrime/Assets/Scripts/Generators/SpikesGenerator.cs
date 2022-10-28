using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikesGenerator : MonoBehaviour, IMiscTileGenerator
{

    public bool enable = false;
    public TileBase tile;
    public float chance;
    public float expandChance = 66;


    public TileBase GetTile() {
        return tile;
    }

    public int[,] Generate(int[,] map, System.Random rng) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] output = new int[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                output[x, y] = 0;
            }
        }
        if (!enable) {
            return output;
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int num = rng.Next(0, 100);
                if (num < chance) {
                    if (output[x,y] == 0 && checkConditions(map,x,y)) {
                        output[x, y] = 1;
                        int totalLength = 1;
                        int inc = 1;
                        while (totalLength <= 5 && rng.Next(0, 100) < expandChance && checkConditions(map, x+inc, y)) {
                            output[x + inc, y] = 1;
                            inc++;
                            totalLength++;
                        }
                        inc = 1;
                        while (totalLength <= 5 && rng.Next(0, 100) < expandChance && checkConditions(map, x - inc, y)) {
                            output[x - inc, y] = 1;
                            inc++;
                            totalLength++;
                        }
                    }
                }
            }
        }

        return output;
    }

    bool checkConditions(int[,] map, int x, int y) {
        int flatPoints = 0;
        if (map[x,y] == 1 || GameMaster.instance.tilemap.GetTile(new Vector3Int(x, y, 0)) != null) {
            return false;
        }
        if (!TouchingGround(map, x, y))
            return false;
        if (!TouchingGround(map, x-1, y)) {
            return false;
        }
        if (!TouchingGround(map, x+1, y)) {
            return false;
        }
        for (int checkX = x - 10; checkX <= x + 10; checkX++) {
            if (checkX >= 0 && checkX < map.GetLength(0)) {
                if (GameMaster.instance.levelGenerator.structureMap != null && GameMaster.instance.levelGenerator.structureMap[checkX, y] == 1) {
                    return false;
                }
            }
        }
        return true;

    }

    bool TouchingGround(int[,] map, int x, int y) {
        return x >= 0 && x < map.GetLength(0) && y >= 1 && y < map.GetLength(1) && map[x, y - 1] == 1;
    }

}
