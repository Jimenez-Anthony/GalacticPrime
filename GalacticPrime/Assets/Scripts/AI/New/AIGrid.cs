using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class AIGrid : MonoBehaviour {

    public Tilemap tilemap;
    public GameObject debug;
    public GameObject debug2;

	public bool displayAIGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 AIGridWorldSize;
	public float nodeRadius;
	public Location[,] aiGrid;

    float nodeDiameter;
	public int AIGridSizeX, AIGridSizeY;

	void Awake() {
		nodeDiameter = nodeRadius*2;
        //AIGridSizeX = Mathf.RoundToInt(AIGridWorldSize.x/nodeDiameter);
        //AIGridSizeY = Mathf.RoundToInt(AIGridWorldSize.y/nodeDiameter);
        AIGridSizeX = tilemap.cellBounds.xMax - tilemap.cellBounds.xMin;
        AIGridSizeY = tilemap.cellBounds.yMax - tilemap.cellBounds.yMin;
        //print(AIGridSizeX + "; " + AIGridSizeY);
        //print(AIGridSizeX + ", " + AIGridSizeY);
        CreateAIGrid();

        if (displayAIGridGizmos)
            DrawDebug();
	}

	public int MaxSize {
		get {
			return AIGridSizeX * AIGridSizeY;
		}
	}

	void CreateAIGrid() {
        aiGrid = new Location[AIGridSizeX,AIGridSizeY];

        //Vector3 worldBottomLeft = transform.position - Vector3.right * AIGridWorldSize.x/2 - Vector3.forward * AIGridWorldSize.y/2;

        //for (int x = 0; x < AIGridSizeX; x ++) {
        //	for (int y = 0; y < AIGridSizeY; y ++) {
        //		Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
        //		bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
        //              aiGrid[x,y] = new Node(walkable,worldPoint, x,y);
        //	}
        //}

        for (int i = 0; i < AIGridSizeX; i += 1) {
            for (int j = 0; j < AIGridSizeY; j += 1) {
                Vector2Int pos = new Vector2Int(i + tilemap.cellBounds.xMin, j + tilemap.cellBounds.yMin);
                bool walkable = tilemap.GetTile(new Vector3Int(pos.x, pos.y, 0)) == null;
                Vector3 worldPoint = tilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0));
                worldPoint = new Vector3(worldPoint.x + 0.5f, worldPoint.y + 0.5f, 0f);
                aiGrid[i, j] = new Location(tilemap, walkable, worldPoint, pos.x, pos.y);
                //aiGrid[i, j].Add(new Node(walkable, worldPoint, pos.x, pos.y));
            }
        }
    }

	public List<Location> GetNeighbours(Location loc) {
        print("Getting neighbors for: " + loc.cellX + ", " + loc.cellY);
		List<Location> neighbours = new List<Location>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = loc.gridX + x;
				int checkY = loc.gridY + y;

				if (checkX >= 0 && checkX < AIGridSizeX && checkY >= 0 && checkY < AIGridSizeY) {
                    //print("neightbor: " + checkX + ", " + checkY);
					neighbours.Add(aiGrid[checkX, checkY]);
				}
			}
		}
        
		return neighbours;
	}
	

	public Location NodeFromWorldPoint(Vector3 worldPosition) {
        //float percentX = (worldPosition.x + AIGridWorldSize.x/2) / AIGridWorldSize.x;
        //float percentY = (worldPosition.z + AIGridWorldSize.y/2) / AIGridWorldSize.y;
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        //int x = Mathf.RoundToInt((AIGridSizeX-1) * percentX);
        //int y = Mathf.RoundToInt((AIGridSizeY-1) * percentY);

        Vector3Int cellLoc = tilemap.WorldToCell(worldPosition);
		return aiGrid[cellLoc.x - tilemap.cellBounds.xMin, cellLoc.y - tilemap.cellBounds.yMin];
	}
	
	void DrawDebug() {
		//Gizmos.DrawWireCube(transform.position,new Vector3(AIGridWorldSize.x,1,AIGridWorldSize.y));
		if (aiGrid != null && displayAIGridGizmos) {
			foreach (Location n in aiGrid) {
				//Gizmos.color = (n.walkable)?Color.white:Color.red;
				//Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
                if (n.walkable) {
                    GameObject clone = Instantiate(debug, n.worldPos, Quaternion.identity) as GameObject;
                    clone.name = n.cellX + ", " + n.cellY;
                }
                else {
                    GameObject clone = Instantiate(debug2, n.worldPos, Quaternion.identity) as GameObject;
                    clone.name = n.cellX + ", " + n.cellY;
                }
            }
		}
	}
}

public class Location : IHeapItem<Location> {
    Tilemap tilemap;
    public int cellX;
    public int cellY;
    public int gridX;
    public int gridY;
    public Vector3 worldPos;
    public bool walkable;
    public List<Node> nodes;
    public int nodeIndex;
    public int gCost;
    public int hCost;
    int heapIndex;

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public Location(Tilemap _tilemap, bool _walkable, Vector3 _worldPos, int _cellX, int _cellY) {
        walkable = _walkable;
        worldPos = _worldPos;
        tilemap = _tilemap;
        cellX = _cellX;
        cellY = _cellY;
        gridX = cellX - tilemap.cellBounds.xMin;
        gridY = cellY - tilemap.cellBounds.yMin;
        nodes = new List<Node>();
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public void ClearNodes() {
        nodes.Clear();
    }

    public int CompareTo(Location nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}