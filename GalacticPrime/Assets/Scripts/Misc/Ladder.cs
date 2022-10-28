using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ladder : MonoBehaviour
{
    public bool bottom = false;
    private int height = 0;
    public RuleTile tile;
    public Tilemap map;


    void Start() {
        map = GameObject.FindObjectOfType<Tilemap>();

        Vector3Int gridLoc = map.WorldToCell(transform.position);
        if (map.GetTile(new Vector3Int(gridLoc.x, gridLoc.y - 1, 0)) != tile) {
            bottom = true;
        }

        if (bottom) {
            height = GetLadderCount();
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.75f, map.cellSize.y * height);
            collider.offset = new Vector2(0f, 0.5f * (map.cellSize.y * height - 1));
        }

    }

    int GetLadderCount() {
        int output = 0;
        Vector3Int gridLoc = map.WorldToCell(transform.position);
        int y = gridLoc.y;
        while (y < map.cellBounds.yMax && map.GetTile(new Vector3Int(gridLoc.x, y, 0)) == tile) {
            output++;
            y++;
        }
        return output;
    }
}
