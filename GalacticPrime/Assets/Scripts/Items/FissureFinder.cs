using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FissureFinder : MonoBehaviour
{

    public float life = 7f;
    public int dir = 0;
    private float timer = 0f;
    public FissuredTile fissuredTile;
    public GameObject fissureParticles;

    private Rigidbody2D rb2d;
    private List<Vector3Int> touchedLocs;
    private Tilemap map;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        rb2d = GetComponent<Rigidbody2D>();
        touchedLocs = new List<Vector3Int>();
        map = GameMaster.instance.tilemap;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < life) {
            timer += Time.deltaTime;
            float yVel = 0f;
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, new Vector3(dir, 0f), 0.5f)) {
                if (hit.transform.tag == "Ground") {
                    yVel = 150f;
                }
            }
            rb2d.velocity = new Vector2(600f * dir * Time.deltaTime, yVel);
        }
        else {
            rb2d.velocity = Vector2.zero;
            rb2d.bodyType = RigidbodyType2D.Static;
            Destroy(gameObject, 1.5f);
        }

        Vector3Int touchedLoc = map.WorldToCell(new Vector3(transform.position.x, transform.position.y - 1f, 0f));
        if (map.GetTile(touchedLoc) == GameMaster.instance.levelGenerator.tile) {
            if (!touchedLocs.Contains(touchedLoc)) {
                touchedLocs.Add(touchedLoc);
                StartCoroutine(FissureAt(touchedLoc));
            }
        }
    }

    IEnumerator FissureAt(Vector3Int loc) {
        map.SetColor(loc, new Color32(255, 255, 255, 0));
        FissuredTile tileClone = Instantiate(fissuredTile, new Vector3(map.CellToWorld(loc).x + 0.5f, map.CellToWorld(loc).y + 0.5f), Quaternion.identity) as FissuredTile;
        tileClone.sprite = map.GetSprite(loc);
        Destroy(tileClone.gameObject, 0.4f);

        GameObject particles = Instantiate(fissureParticles, new Vector3(map.CellToWorld(loc).x + 0.5f, map.CellToWorld(loc).y + 0.5f), fissureParticles.transform.rotation);
        particles.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, map.GetSprite(loc));
        Destroy(particles, 0.5f);

        yield return new WaitForSeconds(0.4f);
        map.SetColor(loc, new Color32(255, 255, 255, 255));
    }
}
