using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JProceduralStructureGenerator : MonoBehaviour {

    public class StructureRoom {
        public Vector2Int roomCoords;
        public bool starting;
        public bool ending;
        public StructureRoom nextRoom;
        public CONNECTDIR connectDir;

        public enum CONNECTDIR { Left, Right, Up, Down };

        public StructureRoom(Vector2Int _roomCoords, bool _starting, bool _ending) {
            roomCoords = _roomCoords;
            starting = _starting;
            ending = _ending;
        }
    }

    public int roomSize = 12;
    public int roomsWidth = 12;
    public int roomsHeight = 6;
    public bool allowReversePath = false;

    private int width;
    private int height;

    private Tilemap tilemap;

    private System.Random rng;

    public Tilemap[] LR_Rooms;
    public Tilemap[] LRD_Rooms;
    public Tilemap[] LRU_Rooms;
    public Tilemap[] LRUD_Rooms;
    public Tilemap blankRoom;

    private int direction;
    private Vector2Int startRoomPos;
    private Vector2Int currentRoomPos;
    private List<StructureRoom> existingRooms;
    private List<Vector2Int> existingCoords;
    private List<Vector2Int> allCoords;
    private StructureRoom previousRoom;
    public int roomCount = 0;
    private bool done = false;

    private bool mainDone = false;
    private int loops = 1;

    void Start() {
        //tilemap = GameMaster.instance.tilemap;
        //existingRooms = new List<Vector2Int>();
        allCoords = new List<Vector2Int>();
        mainDone = false;
    }

    void Update() {
    }

    public void Generate(System.Random rng) {
        if (allCoords == null) {
            allCoords = new List<Vector2Int>();
        }

        this.rng = rng;
        tilemap = GameMaster.instance.tilemap;
        existingRooms = new List<StructureRoom>();
        existingCoords = new List<Vector2Int>();

        currentRoomPos = ChooseStartRoom();
        startRoomPos = currentRoomPos;

        if (loops == 1) {
            width = roomsWidth * roomSize;
            height = roomsHeight * roomSize;
            GameMaster.instance.levelGenerator.map = new int[width, height];
            tilemap.ClearAllTiles();
        }

        done = false;
        print("Starting generation at: " + currentRoomPos.x + " " + currentRoomPos.y);
        previousRoom = new StructureRoom(currentRoomPos, true, false);
        existingRooms.Add(previousRoom);
        existingCoords.Add(previousRoom.roomCoords);
        //allCoords.Add(previousRoom.roomCoords);

        //PlaceRoom(currentRoomPos.x * roomSize, currentRoomPos.y * roomSize, rooms[rng.Next(0, rooms.Length)]);
        direction = 0;

        while (!done) {
            GenerateAdjacentRoom();
        }

        foreach (Vector2Int coord in existingCoords) {
            print("Room: " + coord.x + " " + coord.y);
        }
        SpawnRooms();

        if (!mainDone) {
            GenerateEntrance();
            GenerateExit();
            mainDone = true;
        }

        loops++;
        if (loops < 4) {
            Generate(rng);
        }
        if (loops == 4) {
            FillRemainingRooms();
        }
    }

    Vector2Int ChooseStartRoom() {

        List<Vector2Int> startPoints = new List<Vector2Int>();

        for (int roomX = 0; roomX < roomsWidth; roomX++) {
            for (int roomY = 0; roomY < roomsHeight; roomY++) {
                if (roomY == 0 || roomY == roomsHeight - 1) {
                    startPoints.Add(new Vector2Int(roomX, roomY));
                }
            }
        }

        return startPoints[rng.Next(0, startPoints.Count)];
    }

    void GenerateAdjacentRoom() {
        int dir = GetDirection(direction);
        Vector2Int newRoomPos = new Vector2Int(currentRoomPos.x, currentRoomPos.y);
        if (dir == 1 || dir == 2) {
            if (currentRoomPos.x == roomsWidth - 1) {
                return;
            }
            newRoomPos = new Vector2Int(currentRoomPos.x + 1, currentRoomPos.y);
        }
        else if (dir == 3 || dir == 4) {
            if (currentRoomPos.x == 0) {
                return;
            }
            newRoomPos = new Vector2Int(currentRoomPos.x - 1, currentRoomPos.y);
        }
        else if (dir == 5) {
            newRoomPos = new Vector2Int(currentRoomPos.x, currentRoomPos.y - 1);
        }
        else if (dir == 6) {
            newRoomPos = new Vector2Int(currentRoomPos.x, currentRoomPos.y + 1);
        }

        if (existingCoords.Contains(newRoomPos)) {
            return;
        }

        currentRoomPos = newRoomPos;
        existingCoords.Add(currentRoomPos);

        StructureRoom room = new StructureRoom(currentRoomPos, false, false);
        if (dir == 1 || dir == 2) {
            previousRoom.connectDir = StructureRoom.CONNECTDIR.Right;
        }
        else if (dir == 3 || dir == 4) {
            previousRoom.connectDir = StructureRoom.CONNECTDIR.Left;
        }
        else if (dir == 5) {
            previousRoom.connectDir = StructureRoom.CONNECTDIR.Down;
        }
        else if (dir == 6) {
            previousRoom.connectDir = StructureRoom.CONNECTDIR.Up;
        }
        direction = dir;
        previousRoom.nextRoom = room;
        previousRoom = room;
        existingRooms.Add(previousRoom);

        if (currentRoomPos.y == roomsHeight - 1 && startRoomPos.y == 0) {
            previousRoom.ending = true;
            done = true;
            return;
        }
        if (currentRoomPos.y == 0 && startRoomPos.y == roomsHeight - 1) {
            previousRoom.ending = true;
            done = true;
            return;
        }

    }

    int GetDirection(int previousDirection) {
        int dir = rng.Next(1, 7);
        if (previousDirection == 1 || previousDirection == 2) {
            if (dir == 3 || dir == 4) {
                return GetDirection(previousDirection);
            }
        }
        if (previousDirection == 3 || previousDirection == 4) {
            if (dir == 1 || dir == 2) {
                return GetDirection(previousDirection);
            }
        }
        if (dir == 5) {
            if (previousDirection == 6) {
                return GetDirection(previousDirection);
            }
            if (startRoomPos.y == 0 && !allowReversePath) {
                return GetDirection(previousDirection);
            }
        }
        if (dir == 6) {
            if (previousDirection == 5) {
                return GetDirection(previousDirection);
            }
            if (startRoomPos.y == roomsHeight - 1 && !allowReversePath) {
                return GetDirection(previousDirection);
            }
        }
        return dir;
    }

    void SpawnRooms() {
        //Tilemap startRoom = ChooseRoom(new List<Tilemap[]>{LR_Rooms, LRD_Rooms, LRU_Rooms, LRUD_Rooms});
        //PlaceRoom(existingRooms[0].roomCoords.x * roomSize, existingRooms[0].roomCoords.y * roomSize, startRoom);

        for (int i = 0; i < existingRooms.Count; i++) {
            List<Tilemap[]> possibilities = new List<Tilemap[]> { LR_Rooms, LRD_Rooms, LRU_Rooms, LRUD_Rooms };

            if (existingRooms[i].connectDir == StructureRoom.CONNECTDIR.Down || existingRooms[i].connectDir == StructureRoom.CONNECTDIR.Up) {
                possibilities.Remove(LR_Rooms);
            }
            if (existingRooms[i].connectDir == StructureRoom.CONNECTDIR.Down) {
                possibilities.Remove(LRU_Rooms);
            }
            if (existingRooms[i].connectDir == StructureRoom.CONNECTDIR.Up) {
                possibilities.Remove(LRD_Rooms);
            }

            if (i > 0) {
                if (existingRooms[i - 1].connectDir == StructureRoom.CONNECTDIR.Down || existingRooms[i - 1].connectDir == StructureRoom.CONNECTDIR.Up) {
                    possibilities.Remove(LR_Rooms);

                }
                if (existingRooms[i - 1].connectDir == StructureRoom.CONNECTDIR.Down) {
                    possibilities.Remove(LRD_Rooms);
                }
                if (existingRooms[i - 1].connectDir == StructureRoom.CONNECTDIR.Up) {
                    possibilities.Remove(LRU_Rooms);
                }
                if (existingRooms[i].connectDir == StructureRoom.CONNECTDIR.Down && existingRooms[i - 1].connectDir == StructureRoom.CONNECTDIR.Up) {
                    possibilities.Remove(LRD_Rooms);
                    possibilities.Remove(LRU_Rooms);
                    possibilities.Remove(LR_Rooms);
                }
                if (existingRooms[i].connectDir == StructureRoom.CONNECTDIR.Up && existingRooms[i - 1].connectDir == StructureRoom.CONNECTDIR.Down) {
                    possibilities.Remove(LRD_Rooms);
                    possibilities.Remove(LRU_Rooms);
                    possibilities.Remove(LR_Rooms);
                }
            }
            Tilemap room = ChooseRoom(possibilities);
            //print(existingCoords[i].x);
            if (!allCoords.Contains(existingCoords[i])) {
                allCoords.Add(existingCoords[i]);
                PlaceRoom(existingCoords[i].x * roomSize, existingCoords[i].y * roomSize, room);
            }
        }
    }

    Tilemap ChooseRoom(List<Tilemap[]> possibilities) {
        Tilemap[] chosen = possibilities[rng.Next(0, possibilities.Count)];
        Tilemap chosenTilemap = chosen[rng.Next(0, chosen.Length)];
        return chosenTilemap;
    }

    void PlaceRoom(int startX, int startY, Tilemap room) {
        roomCount++;
        for (int x = 0; x < roomSize; x++) {
            for (int y = 0; y < roomSize; y++) {
                tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), room.GetTile(new Vector3Int(x, y, 0)));
                if (room.GetTile(new Vector3Int(x, y, 0)) == GameMaster.instance.levelGenerator.tile) {
                    //print((startX + x) + " " + (startY + y));
                    //print(GameMaster.instance.levelGenerator.map.GetLength(0) + " " + GameMaster.instance.levelGenerator.map.GetLength(1));
                    GameMaster.instance.levelGenerator.map[startX + x, startY + y] = 1;
                }

                if (startX + x == 0 || startX + x == width - 1 || startY + y == 0 || startY + y == height - 1) {
                    tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), GameMaster.instance.levelGenerator.tile);
                }
            }
        }
    }

    void GenerateEntrance() {
        int startX = startRoomPos.x;
        int startY = startRoomPos.y;
        for (int x = 1; x < roomSize - 1; x++) {
            for (int y = 1; y < roomSize - 1; y++) {
                if (tilemap.GetTile(new Vector3Int(startX * roomSize + x, startY * roomSize + y, 0)) == null) {
                    if (tilemap.GetTile(new Vector3Int(startX * roomSize + x, startY * roomSize + y + 1, 0)) == null) {
                        if (tilemap.GetTile(new Vector3Int(startX * roomSize + x, startY * roomSize + y - 1, 0)) != null) {
                            x = startX * roomSize + x;
                            y = startY * roomSize + y;
                            tilemap.SetTile(new Vector3Int(x, y, 0), GameMaster.instance.levelGenerator.startTile);
                            GameMaster.instance.levelGenerator.map[x, y] = 1;
                            tilemap.SetTile(new Vector3Int(x, y + 1, 0), GameMaster.instance.levelGenerator.startTop);
                            //print("zzzz");
                            GameMaster.instance.levelGenerator.map[x, y + 1] = 1;
                            GameMaster.instance.levelGenerator.map[x, y + 2] = 1;
                            return;
                        }
                    }
                }
            }
        }
    }

    void GenerateExit() {
        int startX = existingCoords[existingCoords.Count - 1].x;
        int startY = existingCoords[existingCoords.Count - 1].y;
        for (int x = 0; x < roomSize; x++) {
            for (int y = 0; y < roomSize; y++) {
                if (tilemap.GetTile(new Vector3Int(startX * roomSize + x, startY * roomSize + y, 0)) == null) {
                    if (tilemap.GetTile(new Vector3Int(startX * roomSize + x, startY * roomSize + y + 1, 0)) == null) {
                        if (tilemap.GetTile(new Vector3Int(startX * roomSize + x, startY * roomSize + y - 1, 0)) != null) {
                            x = startX * roomSize + x;
                            y = startY * roomSize + y;
                            tilemap.SetTile(new Vector3Int(x, y, 0), GameMaster.instance.levelGenerator.endTile);
                            //print(GameMaster.instance.levelGenerator.map.GetLength(0) + " " + GameMaster.instance.levelGenerator.map.GetLength(1));
                            GameMaster.instance.levelGenerator.map[x, y] = 1;
                            tilemap.SetTile(new Vector3Int(x, y + 1, 0), GameMaster.instance.levelGenerator.endTop);
                            //print("zzzz");
                            GameMaster.instance.levelGenerator.map[x, y + 1] = 1;
                            tilemap.SetTile(new Vector3Int(x, y + 2, 0), GameMaster.instance.levelGenerator.doorArrow);
                            GameMaster.instance.levelGenerator.map[x, y + 2] = 1;
                            return;
                        }
                    }
                }
            }
        }
    }

    void FillRemainingRooms() {
        for (int roomX = 0; roomX < roomsWidth; roomX++) {
            for (int roomY = 0; roomY < roomsHeight; roomY++) {
                Vector2Int roomPos = new Vector2Int(roomX, roomY);
                if (!allCoords.Contains(roomPos)) {
                    PlaceRoom(roomX * roomSize, roomY * roomSize, blankRoom);
                }
            }
        }
    }
      
}
