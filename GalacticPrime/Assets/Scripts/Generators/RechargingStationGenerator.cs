using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RechargingStationGenerator : MonoBehaviour, IMiscTileGenerator {

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

        for (int x = 1; x < width - 1; x++) {
            for (int y = 1; y < height - 2; y++) {
                int num = rng.Next(0, 100);
                if (num < chance) {
                    if (map[x, y] == 1 && InGround(map, x, y)) {
                        output[x, y] = 1;
                    }
                }
            }
        }

        return output;
    }

    bool InGround(int[,] map, int x, int y) {
        return map[x, y + 1] == 0 && map[x, y + 2] == 0 && map[x, y - 1] == 1;
    }
}
