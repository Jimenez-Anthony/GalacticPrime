using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnSnake : MonoBehaviour
{

    private Tilemap tilemap;
    public GameObject snake;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameMaster.instance.tilemap;
        Instantiate(snake, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        tilemap.SetTile(tilemap.WorldToCell(transform.position), null);
    }
}
