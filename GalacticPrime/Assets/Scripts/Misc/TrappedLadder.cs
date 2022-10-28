using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrappedLadder : MonoBehaviour
{
    private float delay = 0.1f;
    private float cd = 3f;
    private float timer = 0f;

    public bool controller = false;

    public TileBase spikesStationary;
    public TileBase spikesMoving;
    public TrappedLadder spikesObject;
    private Tilemap map;

    private List<Collider2D> colliders;

    void NewMap() {
        //print("Destroying spike");
        Destroy(this.gameObject);
    }

    void Start() {
        if (controller) {
            map = GameObject.FindObjectOfType<Tilemap>();
            colliders = new List<Collider2D>();
        }
        else {
            TrappedLadder clone = Instantiate(spikesObject, transform.position, Quaternion.identity) as TrappedLadder;
            clone.controller = true;
            clone.transform.parent = transform.parent.parent;
            Destroy(this.gameObject);
        }
    }

    void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        colliders.Add(col);
        if (timer <= 0) {
            StartCoroutine(Activate());
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        colliders.Remove(col);
    }

    IEnumerator Activate() {
        timer = cd;
        yield return new WaitForSeconds(delay);
        Vector3Int gridLoc = map.WorldToCell(transform.position);
        map.SetTile(gridLoc, spikesMoving);
        foreach (Collider2D col in colliders) {
            if (col.tag == "Player" || col.tag == "Enemy") {
                JHealthController health = col.GetComponent<JHealthController>();
                health.TakeDamage(999);
            }
        }
        yield return new WaitForSeconds(0.8f);
        map.SetTile(gridLoc, spikesStationary);
    }
}
