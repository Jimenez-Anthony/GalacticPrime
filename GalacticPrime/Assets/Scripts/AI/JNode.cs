using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JNode : IHeapItem<JNode> {

    public Vector2Int mapPos;
    private Vector3 worldPos;

    public float gCost;
    public float hCost;
    public JNode parent;

    public bool disabled = false;
    public bool air = false;
    public int cost;
    public List<JLink> links;

    int heapIndex;

    public JNode(Vector2Int mapPos, Vector3 worldPos, int cost, bool air = false) {
        links = new List<JLink>();
        this.mapPos = mapPos;
        this.worldPos = worldPos;
        this.cost = cost;
        this.air = air;
    }

    public float fCost {
        get {
            return gCost + hCost;
        }
    }

    public Vector3 getWorldPos() {
        return worldPos;
    }

    public void addLink(JNode node, bool jump = false, bool air = false) {
        links.Add(new JLink(node, jump, air));
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(JNode nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
    
}

public class JLink {
    public JNode dest;
    public bool jump = false;
    public bool airlink = false;

    public JLink(JNode node, bool jump = false, bool air = false) {
        dest = node;
        this.jump = jump;
        this.airlink = air;
    }
}
