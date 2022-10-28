using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JMapGenerator : MonoBehaviour
{
    public bool debugging = false;
    public bool setupAstar = false;
    public GameObject debug;
    public GameObject debug2;
    public Tilemap tilemap;
    public JPathFinder pathfinder;
    public int maxJumpTiles = 3;
    public int nodeSize = 1;

    private List<JNode> nodes;

    private AstarPath astar;

    void Start() {
        //tilemap = GameMaster.instance.tilemap;
    }

    public void SetupPathfinding() {
        tilemap = GameMaster.instance.tilemap;
        nodes = new List<JNode>();
        createNewMap();
        //scanForObstacles();
        addPrimaryLinks();
        addSecondaryLinks();
        //addTertiaryLinks();
        pathfinder.initPathFinder(this, nodes);

        if (setupAstar) {
            astar = AstarPath.active;
            SetupAstar();
        }

        if (debugging) {
            runDebug();
        }
    }

    void createNewMap() {
        for (int i = tilemap.cellBounds.xMin; i < tilemap.cellBounds.xMax; i += nodeSize) {
            for (int j = tilemap.cellBounds.yMin; j < tilemap.cellBounds.yMax; j += nodeSize) {
                Vector2Int pos = new Vector2Int(i, j);
                if (tilemap.GetTile(v3(pos)) != GameMaster.instance.levelGenerator.tile) {
                    bool air = false;
                    if (tilemap.GetTile(v3(yDec(pos))) != GameMaster.instance.levelGenerator.tile) {
                        air = true;
                    }
                    Vector2 Worldpos = tilemap.CellToLocal(new Vector3Int(i, j, 0));
                    Worldpos.x += tilemap.cellSize.x / 2;
                    Worldpos.y += tilemap.cellSize.y / 2;
                    nodes.Add(new JNode(pos, Worldpos, 1, air));
                }
            }
        }
    }

    //void scanForObstacles() {
    //    foreach (JNode n in nodes) {
    //        if (tilemap.GetTile(v3(n.mapPos)) != null) {
    //            //n.disabled = true;
    //        }
    //        //if (tilemap.GetTile(v3(yDec(n.mapPos))) == null) {
    //        //    n.disabled = true;
    //        //}
    //    }
    //}

    void addPrimaryLinks() {
        foreach (JNode n in nodes) {
            if (!n.disabled && !n.air) {
                JNode neighbor = findNode(xDec(n.mapPos));
                if (neighbor != null && neighbor.air == false)
                    n.addLink(neighbor, false);
                neighbor = findNode(xInc(n.mapPos));
                if (neighbor != null && neighbor.air == false)
                    n.addLink(neighbor, false);
            }
        }
    }

    void addSecondaryLinks() {
        foreach (JNode n in nodes) {
            int storedSide = 0;
            for (int w = 0; w < 2; w++) {
                //if (!n.disabled && !n.air && !isEdge(n) && getNeighborCount(n) <= 1) {
                //    //print("Looking for secondary for " + n.mapPos.x + ", " + n.mapPos.y);
                //    int jumpDir = 1;
                //    if (getNeighborCount(n) == 1 && n.links[0].dest.mapPos.x > n.mapPos.x) {
                //        jumpDir = -1;
                //    }
                //    if (storedSide != 0)
                //        jumpDir = -storedSide;
                //    //print(jumpDir);
                //    bool found = false;
                //    for (int x = 1; x <= maxJumpTiles; x++) {
                //        if (found)
                //            break;
                //        for (int y = 1; y <= maxJumpTiles; y++) {
                //            JNode lookFor = findNode(new Vector2Int(n.mapPos.x + jumpDir * x, n.mapPos.y - y));
                //            if (lookFor != null) {
                //                if (lookFor.mapPos.x < n.mapPos.x && tilemap.GetTile(v3(xDec(n.mapPos))) == GameMaster.instance.levelGenerator.tile)
                //                    continue;
                //                if (lookFor.mapPos.x > n.mapPos.x && tilemap.GetTile(v3(xInc(n.mapPos))) != GameMaster.instance.levelGenerator.tile)
                //                    continue;
                //                //print("Added secondary for " + n.mapPos.x + ", " + n.mapPos.y);
                //                //n.addLink(lookFor, false);
                //                lookFor.addLink(n, true);
                //                storedSide = jumpDir;
                //                found = true;
                //                break;
                //            }
                //        }
                //    }
                //    storedSide = jumpDir;
                //    //for (int i = n.mapPos.y - 1; i >= n.mapPos.y - maxJumpTiles; i--) {
                //    //    JNode lookFor = findNode(new Vector2Int(n.mapPos.x + jumpDir, i));
                //    //    if (lookFor != null) {
                //    //        n.addLink(lookFor, false);
                //    //        //lookFor.addLink(n, true);
                //    //        storedSide = jumpDir;
                //    //        break;
                //    //    }
                //    //}
                //}

                if (!n.disabled && !n.air) {
                    for (int x = n.mapPos.x - maxJumpTiles; x <= n.mapPos.x + maxJumpTiles; x++) {
                        for (int y = n.mapPos.y; y <= n.mapPos.y + maxJumpTiles; y++) {
                            if (x == n.mapPos.x && y == n.mapPos.y)
                                continue;
                            if (InBounds(x, y)) {
                                JNode lookFor = findNode(new Vector2Int(x, y));
                                if (lookFor != null && !lookFor.air) {
                                    n.addLink(lookFor, true);
                                }
                            }
                        }
                    }
                }
            }

            if (!n.disabled && !n.air && !isEdge(n)) {
                for (int x = n.mapPos.x - 2; x <= n.mapPos.x + 2; x += 4) {
                    if (tilemap.GetTile(v3(new Vector2Int(n.mapPos.x - 1, n.mapPos.y - 1))) == GameMaster.instance.levelGenerator.tile && tilemap.GetTile(v3(new Vector2Int(n.mapPos.x + 1, n.mapPos.y - 1))) == GameMaster.instance.levelGenerator.tile)
                        break;
                    if (tilemap.GetTile(v3(new Vector2Int(x, n.mapPos.y - 1))) != GameMaster.instance.levelGenerator.tile && tilemap.GetTile(v3(new Vector2Int(x, n.mapPos.y))) != GameMaster.instance.levelGenerator.tile) {
                        for (int i = n.mapPos.y - 1; i >= tilemap.cellBounds.yMin; i--) {
                            JNode lookFor = findNode(new Vector2Int(x, i));
                            if (lookFor != null) {
                                n.addLink(lookFor, false);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    bool InBounds(int x, int y) {
        return x >= tilemap.cellBounds.xMin && x <= tilemap.cellBounds.xMax && y >= tilemap.cellBounds.yMin && y <= tilemap.cellBounds.yMax;
    }

    void addTertiaryLinks() {
        foreach (JNode n in nodes) {
            if (!n.disabled && n.air) {
                if (NextToObstacle(n)) {
                    n.disabled = true;
                }
                for (int x =  - 1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //if (x + y == 1 || x + y == -1) {
                        JNode neighbor = findNode(new Vector2Int(n.mapPos.x + x, n.mapPos.y + y), true);
                        if (neighbor != null && neighbor != n && !NextToObstacle(neighbor)) {
                            n.addLink(neighbor, false, true);
                            if (!neighbor.air) {
                                neighbor.addLink(n, false, true);
                            }
                        }
                        //}
                    }
                }
            }
        }
    }

    void SetupAstar() {
        astar.data.gridGraph.SetDimensions(tilemap.cellBounds.size.x, tilemap.cellBounds.size.y, 1);
        astar.data.gridGraph.center = tilemap.localBounds.center;
        ((Pathfinding.GridGraph)astar.data.graphs[1]).SetDimensions(tilemap.cellBounds.size.x, tilemap.cellBounds.size.y, 1);
        ((Pathfinding.GridGraph)astar.data.graphs[1]).center = tilemap.localBounds.center;


        //astar.UpdateGraphs(tilemap.GetComponent<TilemapCollider2D>().bounds);
        //Physics2D.SyncTransforms();
        StartCoroutine(RunAstar());
        //astar.scan
        print("[AI] Setting up Astar pathfinding");
    }

    IEnumerator RunAstar() {
        yield return null;
        astar.Scan(astar.data.gridGraph);
        astar.Scan(astar.data.graphs[1]);
        tilemap.gameObject.AddComponent<CompositeCollider2D>();
        tilemap.gameObject.GetComponent<TilemapCollider2D>().usedByComposite = true;
        tilemap.gameObject.GetComponent<TilemapCollider2D>().composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
    }

    public JNode GetNodeFarthestFrom(Vector3 pos) {
        float maxDistance = 0f;
        JNode farthestNode = null;
        foreach (JNode n in nodes) {
            if (n.disabled) {
                continue;
            }
            if (!n.air) {
                if (Vector3.Distance(n.getWorldPos(), pos) > maxDistance) {
                    maxDistance = Vector3.Distance(n.getWorldPos(), pos);
                    farthestNode = n;
                }
            }
        }
        return farthestNode;
    }

    // Draw lines for debugging purposes
    void runDebug() {
        foreach (JNode n in nodes) {
            if (n.disabled)
                continue;
            if (!n.air) {
                GameObject clone = Instantiate(debug, n.getWorldPos(), Quaternion.identity) as GameObject;
                clone.name = n.mapPos.x + ", " + n.mapPos.y;
                clone.transform.parent = transform;
            }
            if (n.air) {
                GameObject clone = Instantiate(debug2, n.getWorldPos(), Quaternion.identity) as GameObject;
                clone.name = n.mapPos.x + ", " + n.mapPos.y;
                clone.transform.parent = transform;
            }
        }
    }

    private void OnDrawGizmos() {
        if (!debugging)
            return;
        if (nodes == null)
            return;
        foreach (JNode n in nodes) {
            if (!n.disabled) {
                foreach (JLink link in n.links) {
                    if (link.airlink) {
                        Gizmos.color = Color.yellow;
                    }
                    if (link.jump) {
                        Gizmos.color = Color.blue;
                    }
                    else {
                        Gizmos.color = Color.green;
                    }
                    Vector3 targetPos = link.dest.getWorldPos();
                    Gizmos.DrawLine(n.getWorldPos(), targetPos);
                }
            }
        }
    }

    // Helper
    private Vector3Int v3(Vector2Int v) {
        return new Vector3Int(v.x, v.y, 0);
    }

    private Vector2Int yDec(Vector2Int v) {
        return new Vector2Int(v.x, v.y - 1);
    }

    private Vector2Int yInc(Vector2Int v) {
        return new Vector2Int(v.x, v.y + 1);
    }

    private Vector2Int xDec(Vector2Int v) {
        return new Vector2Int(v.x - 1, v.y);
    }

    private Vector2Int xInc(Vector2Int v) {
        return new Vector2Int(v.x + 1, v.y);
    }

    private Vector3 roundToInt(Vector3 v) {
        return new Vector3((int)v.x, (int)v.y, (int)v.z);
    }

    private bool isEdge(JNode node) {
        return node.mapPos.x == tilemap.cellBounds.xMax - 1 || node.mapPos.x == tilemap.cellBounds.xMin;
    }

    public JNode findNode(Vector2Int loc, bool air = false) {
        foreach (JNode n in nodes) {
            if (air || n.air == false) {
                if (n.mapPos == new Vector2Int(loc.x, loc.y)) {
                    return n;
                }
            }
        }
        return null;
    }

    public JNode findNode(Vector3 loc, bool air = false) {
        JNode output = null;
        foreach (JNode n in nodes) {
            if (air || n.air == false) {
                if (roundToInt(n.getWorldPos()) == roundToInt(loc)) {
                    output = n;
                }
            }
        }
        if (output == null) {
            float closestHeight = float.MaxValue;
            JNode closest = null;
            foreach (JNode n in nodes) {
                if (air || n.air == false) {
                    if ((int)n.getWorldPos().x == (int)loc.x) {
                        if (n.getWorldPos().y < loc.y && loc.y - n.getWorldPos().y < closestHeight) {
                            closest = n;
                            closestHeight = loc.y - n.getWorldPos().y;
                        }
                    }
                }
            }
            output = closest;
        }
        
        return output;
    }

    public bool NextToObstacle(JNode node) {
        for (int x = -1; x <= 1; x++) {

                if (tilemap.GetTile(v3(new Vector2Int(node.mapPos.x + x, node.mapPos.y))) == GameMaster.instance.levelGenerator.tile) {
                    return true;
                }

        }
        return false;
    }

    public List<JNode> getConnectedNodes(JNode node) {
        List<JNode> output = new List<JNode>();
        foreach (JLink link in node.links) {
            output.Add(link.dest);
        }
        return output;
    }

    private int getNeighborCount(JNode node) {
        int count = 0;
        foreach (JLink link in node.links) {
            if (!link.jump)
                count++;
        }
        return count;
    }
    
}
