using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LadderGenerator : MonoBehaviour, IMiscTileGenerator
{
    public bool enable = false;
    public TileBase tile;
    public int minHeightForLadder = 4;
    //public float chance = 100f;
    public bool reduceLadders = true;
    private int[,] output;
    public TileBase trappedTile;
    public float trappedChance = 10f;

    public TileBase GetTile() {
        return tile;
    }

    public int[,] Generate(int[,] map, System.Random rng) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        output = new int[width, height];
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
                if (map[x, y] == 0 && TouchingGround(map, x, y)) {
                    if (x < 1 || x >= width-1 || y < 1 || y >= height-1) {
                        continue;
                    }
                    int side = FindSide(map, x, y - 1);
                    if (side != 0) {
                        if (GetOpenCount(map, x, y, 2) > 12) {
                            Vector3Int startPos = LadderStartPos(map, x + side, y - 1);
                            if (startPos.z >= minHeightForLadder) {
                                if (NoHigherLaddersNearby(x, y, 2, startPos.y) || !reduceLadders) {
                                    for (int ladderY = y; ladderY >= startPos.y; ladderY--) {
                                        output[x + side, ladderY] = 1;
                                    }
                                    if (rng.Next(0, 100) < trappedChance) {
                                        output[x + side, startPos.y] = 2;
                                        GameMaster.instance.tilemap.SetTile(new Vector3Int(x + side, startPos.y, 0), trappedTile);
                                        output[x + side, startPos.y + 1] = 2;
                                        GameMaster.instance.tilemap.SetTile(new Vector3Int(x + side, startPos.y + 1, 0), trappedTile);
                                    }
                                }
                            }
                        }
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

    int FindSide(int[,] map, int x, int y) {
        if (x-1 < 0 || x+1 >= map.GetLength(0) || y-1 < 0 || y+1 >= map.GetLength(1)) {
            return 0;
        }
        if (map[x - 1, y] == 1 && map[x + 1, y] == 1) {
            return 0;
        }
        if (map[x - 1, y] == 1) {
            return 1;
        }
        if (map[x + 1, y] == 1) {
            return -1;
        }
        return 0;
    }

    int GetOpenCount(int[,] map, int x, int y, int radius) {
        int count = 0;
        for (int checkX = x - radius; checkX <= x + radius; checkX++) {
            for (int checkY = y - radius; checkY <= y + radius; checkY++) {
                if (checkX < 0 || checkX >= output.GetLength(0) || checkY < 0 || checkY >= output.GetLength(1)) {
                    continue;
                }

                if (map[checkX,checkY] == 0) {
                    count++;
                }
            }
        }
        return count;
    }

    Vector3Int LadderStartPos(int[,] map, int x, int y) {
        int count = 0;
        int ycheck = y;
        while (ycheck >= 0 && map[x, ycheck] == 0) {
            count++;
            ycheck--;
        }
        return new Vector3Int(x, ycheck + 1, count);
    }

    int AirTilesAbove(int[,] map, int x, int y) {
        int count = 0;
        int ycheck = y + 1;
        while (ycheck < map.GetLength(1) && map[x,ycheck] == 0) {
            count++;
            ycheck++;
        }
        return count;
    }

    int GetLadderHeight(int x, int y) {
        if (output[x,y] == 0) {
            return 0;
        }
        int count = 1;
        int yOffset = 1;
        while (output[x,y + yOffset] >= 1) {
            count++;
            yOffset++;
        }
        yOffset = 1;
        while (y - yOffset >= 0 && output[x,y - yOffset] >= 1) {
            count++;
            yOffset++;
        }
        return count;
    }

    bool NoHigherLaddersNearby(int x, int y, int radius, int startY) {
        //return true;
        for (int checkX = x - radius; checkX <=  x + radius; checkX++) {
            for (int checkY = startY; checkY <= y - 1; checkY++) {
                if (checkX < 0 || checkX >= output.GetLength(0) || checkY < 0 || checkY >= output.GetLength(1)) {
                    continue;
                }
                if (output[checkX, checkY] >= 1) {
                    int height = GetLadderHeight(checkX, checkY);
                    //print(x + ", " + y + " compare: " + height + ", " + (y - startY));
                    if (height < y - startY) {
                        RemoveLadder(checkX, checkY);
                    }
                    else {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    void RemoveLadder(int x, int y) {
        //print("Removing ladder at " + x + ", " + y);
        int yOffset = 0;
        while (output[x, y + yOffset] >= 1) {
            if (output[x, y + yOffset] == 2) {
                GameMaster.instance.tilemap.SetTile(new Vector3Int(x, y + yOffset, 0), null);
            }
            output[x, y + yOffset] = 0;
            yOffset++;
        }
        yOffset = 1;
        while (y - yOffset >= 0 && output[x, y - yOffset] >= 1) {
            if (output[x, y + yOffset] == 2) {
                GameMaster.instance.tilemap.SetTile(new Vector3Int(x, y - yOffset, 0), null);
            }
            output[x, y - yOffset] = 0;
            yOffset++;
        }
    }
}
