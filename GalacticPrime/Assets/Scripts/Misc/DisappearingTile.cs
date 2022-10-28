using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisappearingTile : MonoBehaviour
{

    public static List<DisappearingTile> list = new List<DisappearingTile>();

    private float lifeTime = 5f;
    public float timer;
    private Tilemap map;

    void Start()
    {
        timer = lifeTime;
        map = GameMaster.instance.tilemap;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        Vector3Int loc = map.WorldToCell(transform.position);

        if (timer <= 0f) {
            Destroy(gameObject);
            //map.SetTile(loc, GameMaster.instance.levelGenerator.tile);
            map.SetTile(loc, null);
        }

        //if (map.GetTile(new Vector3Int(loc.x, loc.y - 1, loc.z)) == null && (map.GetTile(new Vector3Int(loc.x, loc.y + 1, loc.z)) == null)) {
        //    map.SetTile(loc, null);
        //    Destroy(gameObject);
        //}



    }
}
