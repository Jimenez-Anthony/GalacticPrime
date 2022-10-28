using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrateGenerator : MonoBehaviour, IMiscTileGenerator
{
    public bool enable = false;
    public TileBase tile;
    public float chance = 1f;

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
                    if (map[x, y] == 0 && TouchingGround(map, x, y)) {
                        output[x, y] = 1;
                    }
                }
            }
        }

        return output;
    }

    bool TouchingGround(int[,] map, int x, int y) {
        if (y - 1 >= 0) {
            return map[x, y - 1] == 1;
        }
        return false;
    }
}
