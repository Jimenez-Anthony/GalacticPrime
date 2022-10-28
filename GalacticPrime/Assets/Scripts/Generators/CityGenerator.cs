using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CityGenerator : MonoBehaviour {

    public static List<Vector3> buildingLocations;

    public int gapSize;
    public int width;
    public int height;
    public int minBuildingHeight;
    public int maxHeightVariation;
    public int maxBuildingWidth;
    public int minBuildingWidth;

    private bool playerSpawned;

    public TileBase groundTile;
    public TileBase spikes;
    public TileBase invisBarrier;
    public TileBase ladder;
    public TileBase exit;
    public TileBase exitTop;
    public TileBase exitSign;
    public GameObject fireworks;

    public GameObject boss;

    private System.Random rng;
    private Tilemap tilemap;

    public static CityGenerator instance;
    void Awake() {
        instance = this;
    }

    void Start() {
        playerSpawned = false;
    }

    void Update() {

    }

    public void Generate(System.Random rng, int seedHash) {
        print("generating city");

        buildingLocations = new List<Vector3>();

        this.rng = rng;
        tilemap = GameMaster.instance.tilemap;
        GameMaster.instance.levelGenerator.map = new int[width, height];
        tilemap.ClearAllTiles();

        int buildingWidth = rng.Next(minBuildingWidth, maxBuildingWidth + 1);
        //print("Setting building widht: " + buildingWidth);
        int c = 0;
        int h = 0;
        int gapCount = 0;
        int previousHeight = rng.Next(minBuildingHeight, minBuildingHeight + maxHeightVariation + 1);
        for (int i = 0; i < width; i++) {

            if (c < buildingWidth) {
                c++;
                h = previousHeight;
                //print(h);
            }
            else {
                h = rng.Next(minBuildingHeight, minBuildingHeight + maxHeightVariation + 1);
                //print("new height: " + h);
                //print(h);
                previousHeight = h;
                c = 0;
                buildingWidth = rng.Next(minBuildingWidth, maxBuildingWidth + 1);
                gapCount = gapSize;
                //print("new buidling width: " + buildingWidth);
            }

            if (gapCount > 0) {
                //h = 0;
                gapCount--;
            }

            if (c == (buildingWidth / 2) + 1 && h != 0) {
                Instantiate(new GameObject(), new Vector3(i, h + 1, 0), Quaternion.identity);
                buildingLocations.Add(new Vector3(i, h + 1, 0));
            }

            float num = Random.Range(0f, 1f);

            for (int y = 0; y < h; y++) {
                //print("Setting tile at " + i + " " + y);
                GameMaster.instance.levelGenerator.map[i, y] = 1;
                if (gapCount == 0) {
                    tilemap.SetTile(new Vector3Int(i, y, 0), GameMaster.instance.levelGenerator.tile);
                }
                else {
                    if (num < 0.5f)
                        tilemap.SetTile(new Vector3Int(i, y, 0), ladder);
                }
            }

            if (c == (buildingWidth / 2) && i > width / 2 && !playerSpawned && h > 0) {
                playerSpawned = true;
                GameMaster.instance.player.transform.position = tilemap.CellToWorld(new Vector3Int(i, h + 1, 0));
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < 6; y++) {
                if (y == 5) {
                    if (tilemap.GetTile(new Vector3Int(x, y, 0)) != GameMaster.instance.levelGenerator.tile) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), spikes);
                    }
                }
                else {
                    tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }

            for (int y = height - 8; y < height; y++) {
                tilemap.SetTile(new Vector3Int(x, y, 0), invisBarrier);
            }

            if (x == 0 || x == width - 1) {
                for (int y = 0; y < height; y++) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), invisBarrier);
                }
            }

        }

        Instantiate(boss, new Vector3(width / 2, 40f, 0f), Quaternion.identity);
    }

    public void OnBossKilled() {
        Instantiate(fireworks, tilemap.CellToWorld(new Vector3Int(width / 2, 10, 0)), fireworks.transform.rotation);

        for (int x = width/2; x < width; x++) {
            for (int y = minBuildingHeight; y < height; y++) {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null) {
                    if (tilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null) {
                        if (tilemap.GetTile(new Vector3Int(x, y - 1, 0)) == GameMaster.instance.levelGenerator.tile) {
                            tilemap.SetTile(new Vector3Int(x, y, 0), exit);
                            tilemap.SetTile(new Vector3Int(x, y+1, 0), exitTop);
                            tilemap.SetTile(new Vector3Int(x, y+2, 0), exitSign);
                            return;
                        }
                    }
                }
            }
        }
    }

    //void SpawnPlayer() {
    //    for (int x = width / 2; x < width; x++) {

    //    }
    //}
}
