using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class JProceduralGenerator : MonoBehaviour
{
    public enum Leveltype {
        CAVE, SURFACE, ASTEROIDS, SEWER, CITY
    }

    public bool generateOnStart = false;
    public bool generateOnClick = false;
    public Leveltype type;
    public Tilemap tilemap;
    public TileBase tile;
    public int width;
    public int height;

    public string seed;
    public bool randomSeed;

    [Space(10)]
    [Header("Cave")]
    [Range(0, 100)]
    public int randomFillPercent;
    public int smoothingIterations = 5;
    public bool processMap = false;
    [Tooltip("Requireds Process Map")]
    public int processThreshhold = 50;
    [Tooltip("Requireds Process Map")]
    public bool connectRooms = false;
    [Tooltip("Requireds Process Map and Connect Rooms")]
    public bool connectionSmoothing = false;
    public bool ensurePath = false;
    public bool ensureJump = false;
    public bool generateBorder = true;

    [Space(10)]
    [Header("Surface")]
    public int baseHeight;
    public float heightVariation;
    public float smoothness;

    [Space(10)]
    [Header("Sewer")]
    private JProceduralStructureGenerator sewerGenerator;

    [Space(10)]
    [Header("City")]
    private CityGenerator cityGenerator;

    [Space(10)]
    [Header("Misc Tiles")]
    public bool runMiscGenerators = false;
    private IMiscTileGenerator[] miscGenerators;

    public int[,] map;
    public System.Random rng;

    // Start and Exit
    [Space(10)]
    public TileBase startTile;
    public TileBase startTop;
    public TileBase endTile;
    public TileBase endTop;
    public TileBase doorArrow;
    public TileBase borderTile;
    public TileBase invisBorder;

    // Structures
    [Space(10)]
    public bool generateStrucutres = true;
    public int[,] structureMap;
    public IStructureGenerator[] structureGenerators;
    public int bossCount = 0;

    void Awake() {
        miscGenerators = gameObject.GetComponents<IMiscTileGenerator>();
        structureGenerators = GetComponents<IStructureGenerator>();
        sewerGenerator = transform.GetChild(0).GetComponent<JProceduralStructureGenerator>();
        cityGenerator = transform.GetChild(1).GetComponent<CityGenerator>();
        tilemap = GameMaster.instance.tilemap;
    }

    void Start() {
        if (generateOnStart)
            GenerateRandomMap();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && generateOnClick) {
            Spikes[] spikes = FindObjectsOfType<Spikes>();
            foreach (Spikes spike in spikes) {
                spike.BroadcastMessage("NewMap", SendMessageOptions.DontRequireReceiver);
            }
            //BroadcastMessage("NewMap", SendMessageOptions.DontRequireReceiver);
            UpdateSeed();
            GenerateMap();
        }
    }

    public void GenerateRandomMap() {
        UpdateSeed();
        GenerateMap();
    }

    void UpdateSeed() {
        if (randomSeed) {
            System.Random randomizer = new System.Random();
            seed = (randomizer.NextDouble() * 1000000).ToString();
            //seed = (Time.time * 100000).ToString();
        }

        rng = new System.Random(seed.GetHashCode());
    }

    void GenerateMap() {
        if (type == Leveltype.CAVE) {
            GenerateCaveMap(rng);
            if (processMap) {
                ProcessCaveMap();
            }
            if (ensurePath) {
                EnsurePath(4);
            }
        }
        else if (type == Leveltype.SURFACE) {
            GenerateSurfaceMap(seed.GetHashCode());
        }
        else if (type == Leveltype.SEWER) {
            sewerGenerator.Generate(rng);
            //return;
        }
        else if (type == Leveltype.CITY) {
            print("city");
            cityGenerator.Generate(rng, seed.GetHashCode());
            return;
        }

        if (type != Leveltype.SEWER && type != Leveltype.CITY) {
            FillTileMap();
        }

        if (generateStrucutres) {
            structureMap = new int[map.GetLength(0), map.GetLength(1)];
            foreach (IStructureGenerator structure in structureGenerators) {
                structure.Generate(structureMap);
            }
            //FillTileMap();

            for (int x = 0; x < structureMap.GetLength(0); x++) {
                for (int y = 0; y < structureMap.GetLength(1); y++) {
                    if (structureMap[x, y] == 1) {
                        map[x, y] = 1;
                    }
                }
            }
        }

        if (runMiscGenerators) {
            GenerateMiscTiles();
        }

        if (type != Leveltype.SEWER) {
            GenerateEntranceAndExit();
        }
    }

    // GENERATOR: CAVE
    void GenerateCaveMap(System.Random rng) {
        map = new int[width, height];
        RandomFillMap(rng);

        for (int i = 0; i < smoothingIterations; i++) {
            SmoothMap();
        }
    }

    void ProcessCaveMap() {
        List<List<Coord>> wallRegions = GetRegions(1);

        int wallThreshSize = processThreshhold;
        foreach (List<Coord> wallRegion in wallRegions) {
            if (wallRegion.Count < wallThreshSize) {
                foreach (Coord tile in wallRegion) {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> emptyRegions = GetRegions(0);

        int emptyThreshSize = processThreshhold;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> emptyRegion in emptyRegions) {
            if (emptyRegion.Count < processThreshhold / 2) {
                foreach (Coord tile in emptyRegion) {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else {
                survivingRooms.Add(new Room(emptyRegion, map));
            }
        }

        if (connectRooms) {
            survivingRooms.Sort();
            survivingRooms[0].main = true;
            survivingRooms[0].accessibleFromMain = true;
            ConnectClosestRooms(survivingRooms);
        }
    }

    void ConnectClosestRooms(List<Room> rooms, bool forceAccessibilityFromMain = false) {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMain) {
            foreach (Room room in rooms) {
                if (room.accessibleFromMain) {
                    roomListB.Add(room);
                }
                else {
                    roomListA.Add(room);
                }
            }
        }
        else {
            roomListA = rooms;
            roomListB = rooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA) {
            if (!forceAccessibilityFromMain) {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0) {
                    continue;
                }
            }

            foreach (Room roomB in roomListB) {
                if (roomA == roomB || roomA.ConnectedTo(roomB)) {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMain) {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMain) {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(rooms, true);
        }

        if (!forceAccessibilityFromMain) {
            ConnectClosestRooms(rooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100f);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line) {
            DrawCircle(c, 5);
        }
        if (connectionSmoothing)
            SmoothMap();
    }

    void DrawCircle(Coord c, int r) {
        r = rng.Next(3, r);
        for (int x = -r; x <= r; x++) {
            for (int y = -r; y <= r; y++) {
                if (x*x + y*y <= r*r) {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (WithinMapRange(drawX, drawY)) {
                        map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    void EnsurePath(int amount) {
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 1; y < map.GetLength(1) - 1; y++) {
                if (map[x,y] == 0) {
                    if (GetEmptyHeight(x, y) < amount && GetEmptyHeight(x, y) > 1) {
                        int yOffset = 0;
                        int rnd = rng.Next(0, 100);
                        if (rnd < 50) {
                            while (WithinMapRange(x, y + yOffset) && map[x, y + yOffset] == 0) {
                                yOffset++;
                            }
                            if (WithinMapRange(x, y + yOffset)) {
                                map[x, y + yOffset] = 0;
                                if (ensureJump && WithinMapRange(x - 1, y + yOffset) && WithinMapRange(x + 1, y + yOffset)) {
                                    map[x - 1, y + yOffset] = 0;
                                    map[x + 1, y + yOffset] = 0;
                                    map[x - 1, y] = 0;
                                    map[x + 1, y] = 0;
                                }
                            }
                        }
                        else {
                            while (WithinMapRange(x, y - yOffset) && map[x, y - yOffset] == 0) {
                                yOffset++;
                            }
                            if (WithinMapRange(x, y - yOffset)) {
                                map[x, y - yOffset] = 0;
                                if (ensureJump && WithinMapRange(x - 1, y - yOffset) && WithinMapRange(x + 1, y - yOffset)) {
                                    map[x - 1, y - yOffset] = 0;
                                    map[x + 1, y - yOffset] = 0;
                                    map[x - 1, y] = 0;
                                    map[x + 1, y] = 0;
                                }
                            }
                        }

                    }
                    //if (GetEmptyHeight(x, y) == amount) {
                    //    Vector2Int lowest = GetLowestSpace(x, y);
                    //    MakeSurroundingsEmpty(lowest.x, lowest.y);
                    //}
                }
            }
        }

        if (ensureJump) {
            SmoothMap();
        }
    }

    int GetEmptyHeight(int x, int y) {
        int count = 1;
        int yOffset = 1;
        while (WithinMapRange(x, y + yOffset) && map[x, y + yOffset] == 0) {
            count++;
            yOffset++;
        }
        yOffset = 1;
        while (WithinMapRange(x, y - yOffset) && map[x, y - yOffset] == 0) {
            count++;
            yOffset++;
        }
        return count;
    }

    Vector2Int GetLowestSpace(int x, int y) {
        int yOffset = 1;
        while (WithinMapRange(x, y - yOffset) && map[x, y - yOffset] == 0) {
            yOffset++;
        }
        return new Vector2Int(x, y - (yOffset - 1));
    }

    void MakeSurroundingsEmpty(int x, int y) {
        for (int adX = x - 1; adX <= x + 1; adX += 2) {
            for (int adY = y; adX <= y + 2; adY++) {
                if (WithinMapRange(adX, adY)) {
                    map[adX, adY] = 0;
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to) {
        List <Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int dir = Math.Sign(dx);
        int slopeDir = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest) {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            dir = Math.Sign(dy);
            slopeDir = Math.Sign(dx);
        }

        int slopeAccumulation = longest / 2;
        for (int i = 0; i < longest; i++) {
            line.Add(new Coord(x, y));
            if (inverted) {
                y += dir;
            }
            else {
                x += dir;
            }

            slopeAccumulation += shortest;
            if (slopeAccumulation >= longest) {
                if (inverted) {
                    x += slopeDir;
                }
                else {
                    y += slopeDir;
                }
            }
            slopeAccumulation -= longest;
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile) {
        return tilemap.CellToWorld(new Vector3Int(tile.tileX, tile.tileY, 0));
    }

    List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 1; x < width; x++) {
            for (int y = 1; y < height; y++) {
                if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion) {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                    if ((y == tile.tileY || x == tile.tileX) && WithinMapRange(x, y)) {
                        if (mapFlags[x, y] == 0 && map[x,y] == tileType) {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool WithinMapRange(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void RandomFillMap(System.Random rng) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1;
                }
                else {
                    map[x, y] = (rng.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neightborWallTiles = GetSurroundingWallCount(x, y);
                if (neightborWallTiles > 4)
                    map[x, y] = 1;
                else if (neightborWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int x = gridX - 1; x <= gridX + 1; x++) {
            for (int y = gridY - 1; y <= gridY + 1; y++) {
                if (WithinMapRange(x, y)) {
                    if (x != gridX || y != gridY) {
                        if (map[x, y] == 1) {
                            wallCount++;
                        }
                    }
                }
                else {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    // GENERATOR: SURFACE
    void GenerateSurfaceMap(long seedHash) {
        map = new int[width, height];
        for (int i = 0; i < width; i++) {
            int h = Mathf.RoundToInt(Mathf.PerlinNoise(seedHash, i / smoothness) * heightVariation) + baseHeight;
            for (int j = 0; j < h; j++) {
                map[i, j] = 1;
            }
        }
    }

    void FillTileMap() {
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (map[x, y] == 1) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
                if (generateBorder && type == Leveltype.CAVE) {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                        map[x, y] = 1;
                        tilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
                    }
                }
                else if (type == Leveltype.SURFACE) {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                        map[x, y] = 1;
                        tilemap.SetTile(new Vector3Int(x, y, 0), invisBorder);
                    }
                }
            }
        }
    }

    // Start and Exit
    void GenerateEntranceAndExit() {
        Vector2Int startLoc = GenerateStart();
        GameMaster.instance.mobManager.startPos = new Vector3Int(startLoc.x, startLoc.y, 0);
        Vector2Int endLoc = FindFurthestEligibleTile(startLoc.x, startLoc.y);
        tilemap.SetTile(new Vector3Int(endLoc.x, endLoc.y, 0), endTile);
        map[endLoc.x, endLoc.y] = 1;
        tilemap.SetTile(new Vector3Int(endLoc.x, endLoc.y + 1, 0), endTop);
        map[endLoc.x, endLoc.y + 1] = 1;
        tilemap.SetTile(new Vector3Int(endLoc.x, endLoc.y + 2, 0), doorArrow);
        map[endLoc.x, endLoc.y + 2] = 1;
    }

    Vector2Int GenerateStart() {
        if (rng.Next(0, 100) < 50 || type == Leveltype.SURFACE) {
            for (int y = height - 1; y >= 0; y--)  {
                for (int x = 0; x < width; x++) {
                    if (EligibleForDoor(x, y)) {
                        int chance = 50;
                        if (rng.Next(0, 100) < chance || type == Leveltype.SURFACE) {
                            tilemap.SetTile(new Vector3Int(x, y, 0), startTile);
                            map[x, y] = 1;
                            tilemap.SetTile(new Vector3Int(x, y + 1, 0), startTop);
                            //print("zzzz");
                            map[x, y + 1] = 1;
                            return new Vector2Int(x, y);
                        }
                        else {
                            chance++;
                        }
                    }
                }
            }
        }
        else {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (EligibleForDoor(x, y)) {
                        int chance = 50;
                        if (rng.Next(0, 100) < chance || type == Leveltype.SURFACE) {
                            tilemap.SetTile(new Vector3Int(x, y, 0), startTile);
                            map[x, y] = 1;
                            tilemap.SetTile(new Vector3Int(x, y + 1, 0), startTop);
                            //print("zzzz");
                            map[x, y + 1] = 1;
                            return new Vector2Int(x, y);
                        }
                        else {
                            chance++;
                        }
                    }
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    Vector2Int FindFurthestEligibleTile(int startx, int starty) {
        int[,] distances = new int[width, height];
        int[,] mapFlags = new int[width, height];
        int tileType = 0;

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startx, starty));
        mapFlags[startx, starty] = 1;
        distances[startx, starty] = 0;

        int distance = 0;

        while (queue.Count > 0) {
            distance++;
            Coord tile = queue.Dequeue();

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                    if ((y == tile.tileY || x == tile.tileX) && WithinMapRange(x, y)) {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
                            mapFlags[x, y] = 1;
                            distances[x, y] = distance;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        int maxDistance = 0;
        Coord max = new Coord(-1, -1);
        for (int i = 0; i < distances.GetLength(0); i++) {
            for (int j = 0; j < distances.GetLength(1); j++) {
                if (!WithinMapRange(i, j)) {
                    continue;
                }
                if (distances[i, j] != 0 && EligibleForDoor(i, j) && distances[i, j] > maxDistance) {
                    maxDistance = distances[i, j];
                    max = new Coord(i, j);
                }
            }
        }

        return new Vector2Int(max.tileX, max.tileY);
    }

    bool EligibleForDoor(int x, int y) {
        if (x - 1 < 0 || x + 1 >= width || y - 1 < 0 || y + 3 >= height) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x,y,0)) != null) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x, y -1, 0)) != tile) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x, y + 1, 0)) != null) {
            return false;
        }
        if (tilemap.GetTile(new Vector3Int(x, y + 2, 0)) != null) {
            return false;
        }
        //if (!TouchingGround(x + 1, y)) {
        //    return false;
        //}
        //if (map[x, y + 1] == 1) {
        //    return false;
        //}
        //if (map[x + 1, y + 1] == 1) {
        //    return false;
        //}
        return true;
    }



    // GENERATORS: MISC TILES
    void GenerateMiscTiles() {
        foreach (IMiscTileGenerator generator in miscGenerators) {
            int[,] locations = generator.Generate(map, rng);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (locations[x, y] == 1) {
                        if (generator.GetTile() != null)
                            tilemap.SetTile(new Vector3Int(x, y, 0), generator.GetTile());
                    }
                }
            }
        }
    }

    bool TouchingGround(int gridX, int gridY) {
        return tilemap.GetTile(new Vector3Int(gridX, gridY - 1, 0)) != null;
    }

    struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }
    }

    class Room : IComparable<Room> {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool accessibleFromMain;
        public bool main;

        public Room() {}

        public Room(List<Coord> roomTiles, int[,] map) {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            foreach (Coord tile in tiles) {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY; y <= tile.tileY; y++) {
                        if (x == tile.tileX || y == tile.tileY) {
                            if (map[x,y] == 1) {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccesibleFromMain() {
            if (!accessibleFromMain) {
                accessibleFromMain = true;
                foreach (Room connectedRoom in connectedRooms) {
                    connectedRoom.accessibleFromMain = true;
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            if (roomA.accessibleFromMain) {
                roomB.SetAccesibleFromMain();
            }
            else if (roomB.accessibleFromMain) {
                roomA.SetAccesibleFromMain();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool ConnectedTo(Room otherRoom) {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom) {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}
