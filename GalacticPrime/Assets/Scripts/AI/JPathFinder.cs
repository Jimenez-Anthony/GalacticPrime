using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;

public class JPathFinder : MonoBehaviour
{
    private List<JNode> nodes;
    public JMapGenerator map;
    private JPathRequestManager manager;

    void Awake() {
        manager = GetComponent<JPathRequestManager>();
    }

    public void initPathFinder(JMapGenerator map, List<JNode> nodes) {
        this.nodes = nodes;
        this.map = map;

        //foreach (JLink l in findPath(startTest.position, endTest.position)) {
        //    Debug.Log(l.dest.mapPos.x + ", " + l.dest.mapPos.y);
        //}
    }

    public void StartFindPath(Vector3 start, Vector3 target, bool flying = false) {
        StartCoroutine(FindPath(start, target, flying));
    }

    public IEnumerator FindPath(Vector3 start, Vector3 end, bool flying = false) {

        //// Diagnostics
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        JNode startNode = map.findNode(start, flying);
        JNode endNode = map.findNode(end, flying);

        if (startNode == null || endNode == null) {
            manager.FinishedProcessing(waypoints, pathSuccess);
            yield break;
        }

        //Debug.Log("Start: " + startNode.mapPos.x + ", " + startNode.mapPos.y);
        //Debug.Log("End: " + endNode.mapPos.x + ", " + endNode.mapPos.y);

        List<JLink> path = new List<JLink>();

        //JNode currentNode = startNode;

        //while (currentNode != endNode) {
        //bool prioritizeJump = false;
        //if (endNode.mapPos.y > currentNode.mapPos.y)
        //    prioritizeJump = true;

        //JLink closest =  null;
        //JNode cloestNode = null;
        //float shortestdistance = float.MaxValue;
        //if (prioritizeJump) {
        //    foreach (JLink link in currentNode.links) {
        //        if (link.jump) {
        //            float d = Vector2Int.Distance(link.dest.mapPos, endNode.mapPos);
        //            if (d < shortestdistance) {
        //                shortestdistance = d;
        //                closest = link;
        //                cloestNode = link.dest;
        //            }
        //        }
        //    }
        //}
        //if (closest == null || !prioritizeJump) {
        //    foreach (JLink link in currentNode.links) {
        //        float d = Vector2Int.Distance(link.dest.mapPos, endNode.mapPos);
        //        if (d < shortestdistance) {
        //            shortestdistance = d;
        //            closest = link;
        //            cloestNode = link.dest;
        //        }
        //    }
        //}

        //path.Add(closest);
        //currentNode = cloestNode;
        //}

        Heap<JNode> openSet = new Heap<JNode>(nodes.Count);
        HashSet<JNode> closedSet = new HashSet<JNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            JNode currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            if (currentNode == endNode) {
                //sw.Stop();
                //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                pathSuccess = true;
                break;
            }

            foreach (JLink link in currentNode.links) {
                //print(link.dest.mapPos.x + ", " + link.dest.mapPos.y);
                if (!flying && link.airlink) {
                    continue;
                }
                if (flying && link.jump) {
                    continue;
                }
                JNode dest = link.dest;
                //print(dest.mapPos.x + ", " + dest.mapPos.y);
                if (dest.disabled || closedSet.Contains(dest)) {
                    continue;
                }

                float newMoveCost = currentNode.gCost + Distance(currentNode, dest);
                if (newMoveCost < dest.gCost || !openSet.Contains(dest)) {
                    dest.gCost = newMoveCost;
                    dest.hCost = Distance(dest, endNode);
                    //path.Add(link);
                    dest.parent = currentNode;

                    if (!openSet.Contains(dest)) {
                        openSet.Add(dest);
                    }
                }
            }

        }

        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath(startNode, endNode, flying);
        }
        manager.FinishedProcessing(waypoints, pathSuccess);
        //return path;
    }

    private Vector3[] RetracePath(JNode startNode, JNode endNode, bool flying) {
        List<JNode> path = new List<JNode>();
        JNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path, flying);
        Array.Reverse(waypoints);
        return waypoints;
    }

    private Vector3[] SimplifyPath(List<JNode> path, bool flying) {
        List<Vector3> waypoints = new List<Vector3>();

        //if (!flying) {
            for (int i = 0; i < path.Count; i++) {
                waypoints.Add(path[i].getWorldPos());
            }
        //}
        //else {
            //Vector2 directionOld = Vector2.zero;
        //if (path.Count != 0) {
        //    waypoints.Add(path[0].getWorldPos());
        //}
            //for (int i = 1; i < path.Count; i++) {
            //    Vector2 directionNew = new Vector2(path[i - 1].mapPos.x - path[i].mapPos.x, path[i - 1].mapPos.y - path[i].mapPos.y);
            //    if (directionNew != directionOld) {
            //        waypoints.Add(path[i].getWorldPos());
            //    }
            //    directionOld = directionNew;
            //}
        //}
        return waypoints.ToArray();
    }

    private float Distance(JNode a, JNode b) {
        return Vector2Int.Distance(a.mapPos, b.mapPos);
    }

    //private float GCost(JNode node, JNode start) {
    //    return Distance(start, node);
    //}

    //private float HCost(JNode node, JNode end) {
    //    return Distance(node, end);
    //}

    //private float FCost(JNode node, JNode start, JNode end) {
    //    return Distance(start, node) + Distance(node, end);
    //}
}
