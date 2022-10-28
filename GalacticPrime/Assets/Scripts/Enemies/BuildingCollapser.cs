using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingCollapser : MonoBehaviour
{

    public TileBase buildingTile;
    private Tilemap tilemap;

    private bool collapsing;
    public ParticleSystem particles;
    private SpriteRenderer sprite;
    private Animator anim;
    private Rigidbody2D rb2d;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collapsing = false;
    }

    void Update()
    {
        
    }


    void OnTriggerStay2D(Collider2D collision) {
        if (collapsing) {
            return;
        }

        //print(collision.tag);

        tilemap = GameMaster.instance.tilemap;
        if (collision.gameObject.tag == "Ground") {
            print(tilemap.GetTile(tilemap.WorldToCell(transform.position)));
            if (tilemap.GetTile(tilemap.WorldToCell(transform.position)) == buildingTile) {
                collapsing = true;
                print("begin building collapse");
                Destroy(rb2d);
                StartCoroutine(StartCollapse(tilemap.WorldToCell(transform.position)));
            }
        }
    }

    IEnumerator StartCollapse(Vector3Int loc) {
        loc = FindHighestTile(loc);
        anim.SetTrigger("Impact");
        yield return new WaitForSeconds(0.5f);
        sprite.enabled = false;
        while (GetBuildingHeight(loc) > 0 && tilemap.GetTile(loc) == buildingTile) {
            foreach (Vector3Int tileCoord in GetTilesInRow(loc)) {
                tilemap.SetTile(tileCoord, null);
                Vector3 worldCoord = tilemap.CellToWorld(tileCoord);
                ParticleSystem particleClone = Instantiate(particles, new Vector3(worldCoord.x + 0.5f, worldCoord.y + 0.5f, 0f), particles.transform.rotation) as ParticleSystem;
                particleClone.textureSheetAnimation.SetSprite(0, ((RuleTile)buildingTile).m_DefaultSprite);
                Destroy(particleClone.gameObject, 0.5f);
            }
            loc = new Vector3Int(loc.x, loc.y - 1, 0);

            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }

    int GetBuildingWidth(Vector3Int loc) {
        int count = 1;
        int x = loc.x - 1;
        while (tilemap.GetTile(new Vector3Int(x, loc.y, 0)) == buildingTile) {
            count++;
            x--;
        }

        x = loc.x - 1;
        while (tilemap.GetTile(new Vector3Int(x, loc.y, 0)) == buildingTile) {
            count++;
            x++;
        }

        return count;
    }

    Vector3Int[] GetTilesInRow(Vector3Int loc) {
        int width = GetBuildingWidth(loc);
        Vector3Int[] coords = new Vector3Int[width];
        coords[0] = loc;

        int index = 1;
        int x = loc.x - 1;
        while (tilemap.GetTile(new Vector3Int(x, loc.y, 0)) == buildingTile) {
            if (Random.Range(0f, 1f) < 0.75f) {
                coords[index] = new Vector3Int(x, loc.y, 0);
            }
            index++;
            x--;
        }

        x = loc.x - 1;
        while (tilemap.GetTile(new Vector3Int(x, loc.y, 0)) == buildingTile) {
            if (Random.Range(0f, 1f) < 0.75f) {
                coords[index] = new Vector3Int(x, loc.y, 0);
            }
            x++;
            index++;
        }

        return coords;
    }

    Vector3Int FindHighestTile(Vector3Int loc) {
        while (tilemap.GetTile(new Vector3Int(loc.x, loc.y + 1, 0)) == buildingTile) {
            loc = new Vector3Int(loc.x, loc.y + 1, 0);
        }
        return loc;
    }

    int GetBuildingHeight(Vector3Int loc) {
        int count = 1;
        int y = loc.y - 1;
        while (tilemap.GetTile(new Vector3Int(loc.x, y, 0)) == buildingTile) {
            count++;
            y--;
        }
        return count;
    }
}
