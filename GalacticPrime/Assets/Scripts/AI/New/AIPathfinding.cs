using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AIPathfinding : MonoBehaviour {
	
	PathRequestManager requestManager;
	AIGrid grid;
    private Stack<Location> touchedLocations;
    int characterHeight = 1;
    int jumpLength = 3;
    int jumpHeight = 3;

    void Start() {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<AIGrid>();
        touchedLocations = new Stack<Location>(grid.AIGridSizeX * grid.AIGridSizeY);
	}
	
	
	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
		StartCoroutine(FindPath(startPos,targetPos));
	}
	
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        while (touchedLocations.Count > 0) {
            Location loc = touchedLocations.Pop();
            grid.aiGrid[loc.gridX, loc.gridY].ClearNodes();
        }

        Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		
		Location startNode = grid.NodeFromWorldPoint(startPos);
		Location targetNode = grid.NodeFromWorldPoint(targetPos);

        //print(targetPos.x + ", " + targetPos.y);

        print("start: " + startNode.cellX + ", " + startNode.cellY);
        print("end: " + targetNode.cellX + ", " + targetNode.cellY);

        print("Try to find path " + startNode.walkable + " " + targetNode.walkable);
        if (startNode.walkable && targetNode.walkable) {
			Heap<Location> openSet = new Heap<Location>(grid.MaxSize);
			HashSet<Location> closedSet = new HashSet<Location>();
            Node startNodeNode = new Node(startNode, true, startNode.worldPos, startNode.gridX, startNode.gridY);
            print(startNode.gridX + ", " + startNode.gridY);
            if (grid.aiGrid[startNode.gridX, startNode.gridY - 1].walkable == false) {
                startNodeNode.jValue = 0;
            }
            else {
                startNodeNode.jValue = 3 * 2;
            }

            print("Start Jvalue: " + startNodeNode.jValue);

            startNode.nodes.Add(startNodeNode);
            startNode.nodeIndex = startNode.nodes.IndexOf(startNodeNode);
            touchedLocations.Push(startNode);
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Location currentNode = openSet.RemoveFirst();
                Node currentNodeNode = currentNode.nodes[currentNode.nodeIndex];
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
                    print("path success");
					pathSuccess = true;
					break;
				}
				
				foreach (Location neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable) {
						continue;
					}

                    print("Found neightbor: " + neighbour.cellX + ", " + neighbour.cellY);

                    bool onGround = false;
                    bool onCeiling = false;

                    if (grid.aiGrid[neighbour.gridX, neighbour.gridY - 1].walkable == false) {
                        print(neighbour.cellX + ", " + neighbour.cellY + " Node on ground");
                        onGround = true;
                    }
                    if (grid.aiGrid[neighbour.gridX, neighbour.gridY + characterHeight].walkable == false) {
                        print(neighbour.cellX + ", " + neighbour.cellY + " Node on ceiling");
                        onCeiling = true;
                    }

                    int jumpValue = currentNodeNode.jValue;
                    int newJumpValue = jumpValue;

                    if (onGround) {
                        newJumpValue = 0;
                    }
                    else if (onCeiling) {
                        if (neighbour.cellX != currentNode.cellX) {
                            newJumpValue = Mathf.Max(jumpHeight * 2 + 1, jumpValue + 1);
                        }
                        else {
                            newJumpValue = Mathf.Max(jumpHeight * 2, jumpValue + 2);
                        }
                    }
                    else if (neighbour.cellY > currentNode.cellY) {
                        if (jumpValue < 2) {
                            newJumpValue = 3;
                        }
                        else if (jumpValue % 2 == 0)
                            newJumpValue = (jumpValue + 2);
                        else
                            newJumpValue = (jumpValue + 1);
                    }
                    else if (neighbour.cellY < currentNode.cellY) {
                        if (jumpValue % 2 == 0)
                            newJumpValue = Mathf.Max(jumpHeight * 2, jumpValue + 2);
                        else
                            newJumpValue = Mathf.Max(jumpHeight * 2, jumpValue + 1);
                    }

                    if (jumpValue % 2 != 0 && currentNode.cellX != neighbour.cellX)
                        continue;

                    if (jumpValue >= jumpHeight * 2 && neighbour.cellY > currentNode.cellY)
                        continue;

                    if (newJumpValue >= jumpHeight * 2 + 6 && neighbour.cellX != currentNode.cellX && (newJumpValue - (jumpHeight * 2 + 6)) % 8 != 3)
                        continue;

                    print("jump value: " + newJumpValue);

                    int newMovementCostToNeighbour = currentNodeNode.gCost + GetDistance(currentNode, neighbour) + newJumpValue;

                    print("cost: " + newMovementCostToNeighbour + " old cost: " + currentNodeNode.gCost);

                    if (neighbour.nodes.Count > 0) {
                        int lowestJump = int.MaxValue;
                        bool couldMoveSideways = false;

                        for (int j = 0; j < neighbour.nodes.Count; ++j) {
                            if (neighbour.nodes[j].jValue < lowestJump)
                                lowestJump = neighbour.nodes[j].jValue;

                            if (neighbour.nodes[j].jValue % 2 == 0 && neighbour.nodes[j].jValue < jumpHeight * 2 + 6)
                                couldMoveSideways = true;
                        }

                        if (lowestJump <= newJumpValue && (newJumpValue % 2 != 0 || newJumpValue >= jumpHeight * 2 + 6 || couldMoveSideways))
                            continue;
                    }

                    if (neighbour.nodes.Count == 0) {
                        print("No nodes found");
                        touchedLocations.Push(neighbour);
                    }

                    if (neighbour.nodes.Count == 0 || newMovementCostToNeighbour < neighbour.nodes[neighbour.nodeIndex].gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
                        Node node = new Node(neighbour, true, neighbour.worldPos, neighbour.gridX, neighbour.gridY);
                        node.jValue = newJumpValue;
						node.parent = currentNodeNode;
                        neighbour.nodes.Add(node);
                        neighbour.nodeIndex = neighbour.nodes.IndexOf(node);
                        print("Added to path: " + neighbour.cellX + ", " + neighbour.cellY);

                        if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath(startNode.nodes[0],targetNode.nodes[0]);
		}
		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
		
	}
	
	Vector3[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
		
	}
	
	Vector3[] SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}
	
	int GetDistance(Location nodeA, Location nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
	
	
}
