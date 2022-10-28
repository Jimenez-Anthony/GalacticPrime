using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DartTrapGenerator : MonoBehaviour, IMiscTileGenerator
{
    public bool enable = false;
    public TileBase leftTile;
    public TileBase rightTile;
    public float chance;

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

        for (int x = 1; x < width - 1; x++) {
            for (int y = 1; y < height - 1; y++) {
                int num = rng.Next(0, 100);
                if (num < chance) {
                    int condition = CheckConditions(map, x, y);
                    if (output[x, y] == 0 && condition != 0) {
                        output[x, y] = 1;
                        if (condition == 1) {
                            GameMaster.instance.tilemap.SetTile(new Vector3Int(x, y, 0), rightTile);
                        }
                        else {
                            GameMaster.instance.tilemap.SetTile(new Vector3Int(x, y, 0), leftTile);
                        }
                    }
                }
            }
        }

        return output;


    }

    public TileBase GetTile() {
        return null;
    }

    int CheckConditions(int[,] map, int x, int y) {
        if (map[x, y] == 0) {
            return 0;
        }
        if (map[x, y + 1] == 0 && map[x, y - 1] == 0) {
            return 0;
        }
        if (map[x + 1, y] == 0 && map[x - 1, y] == 0) {
            return 0;
        }
        if (map[x + 1, y] == 1 && map[x - 1, y] == 1) {
            return 0;
        }
        for (int checkX = x - 10; checkX <= x + 10; checkX++) {
            if (checkX >= 0 && checkX < map.GetLength(0)) {
                if (GameMaster.instance.levelGenerator.structureMap[checkX, y] == 1) {
                    return 0;
                }
            }
        }
        if (map[x - 1, y] == 1) {
            return 1;
        }
        else {
            return -1;
        }

    }
}
