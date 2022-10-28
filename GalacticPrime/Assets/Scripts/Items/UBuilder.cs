using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UBuilder : MonoBehaviour, IItem {
    private JPlayerController playerCont;
    private Vector3 targetPos;
    private Tilemap map;
    public TileBase tile;

    private float builtTime = 0f;
    private List<Vector2Int> builtLocs;

    private float markerDistance = 0f;
    private int dir;

    public GameObject markerPrefab;
    private GameObject marker;
    private bool built;
    private bool charging;



    // Start is called before the first frame update
    void Start() {
        builtLocs = new List<Vector2Int>();
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
        map = GameMaster.instance.tilemap;
        built = false;
        charging = false;
    }

    // Update is called once per frame
    void Update() {
        GameMaster.instance.inventory.UpdateCooldown(GetCooldown());

        if (built) {
            builtTime += Time.deltaTime;
        }

        dir = 0;
        if (GameMaster.instance.player.transform.position.x - transform.position.x > 0) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        if (charging) {
            if (Input.GetKey(Keys.useItem) && markerDistance < 8f) {
                markerDistance += Time.deltaTime * 10f;
                marker.transform.position = new Vector3(transform.position.x + markerDistance * dir, transform.position.y);
            }
            if (Input.GetKeyUp(Keys.useItem)) {
                charging = false;
                print("build wall");
                StartCoroutine(BuildWallAt(marker.transform.position));
                Destroy(marker);
                marker = null;
                transform.parent = null;
                GetComponent<SpriteRenderer>().enabled = false;
                GameMaster.instance.inventory.UseReusableItem();

            }
        }


    }

    public void Use() {
        markerDistance = 0f;
        charging = true;
        marker = Instantiate(markerPrefab, transform, false);
    }

    void OnDestroy() {
        if (marker != null) {
            Destroy(marker);
        }
    }

    IEnumerator BuildWallAt(Vector3 pos) {
        Vector3Int cellPos = map.WorldToCell(pos);
        bool deadTop = false;
        bool deadBot = false;
        int yOffset = 0;
        while (!(deadBot && deadTop)) {
            print("looping");
            if (yOffset > 10) {
                Destroy(gameObject);
                yield break;
            }
            if (deadBot && deadTop) {
                Destroy(gameObject);
                yield break;
            }

            if (!deadTop && map.GetTile(new Vector3Int(cellPos.x, cellPos.y + yOffset, 0)) == null) {
                map.SetTile(new Vector3Int(cellPos.x, cellPos.y + yOffset, 0), tile);
            }
            else {
                deadTop = true;
            }

            if (yOffset != 0 && !deadBot && map.GetTile(new Vector3Int(cellPos.x, cellPos.y - yOffset, 0)) == null) {
                map.SetTile(new Vector3Int(cellPos.x, cellPos.y - yOffset, 0), tile);
            }
            else if (yOffset != 0) {
                deadBot = true;
            }
            yOffset++;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public float GetCooldown() {
        return 0f;
    }

}
