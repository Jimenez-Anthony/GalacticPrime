using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Crate : MonoBehaviour, IDeathController
{
    public bool golden = false;

    public ItemStack tutorialPistol;
    public GameObject destructionParticles;
    public GameObject crateProjectile;
    private Tilemap tilemap;
    public DroppedItem droppedItem;
    public ItemStack drop;

    private bool getRemoved = false;

    private float heldDuration;

    public void OnDeath() {
        //GetComponent<AudioSource>().Play();
        //print("crate death");
        GameObject particles = Instantiate(destructionParticles, transform.position, transform.localRotation) as GameObject;
        Destroy(particles, 0.5f);
        getRemoved = true;
    }

    private void Start() {
        transform.parent = transform.parent.parent;
        tilemap = GameMaster.instance.tilemap;
        if (LevelManager.instance.world == 0) {
            drop = new ItemStack(tutorialPistol.item, tutorialPistol.count, tutorialPistol.durability);
            return;
        }

        ItemStack loot;
        //System.Random rng = GameMaster.instance.levelGenerator.rng;
        //int itemIndex = rng.Next(0, GameMaster.instance.crateDrops.Length);
        if (!golden) { 
            loot = GameMaster.instance.lootManager.GetItem(GameMaster.instance.lootManager.crate);
        }
        else {
            loot = GameMaster.instance.lootManager.GetItem(GameMaster.instance.lootManager.goldenCrate);
        }
        drop = new ItemStack(loot.item, loot.count, loot.item.durability);
        //drop = new ItemStack(GameMaster.instance.crateDrops[itemIndex].item, GameMaster.instance.crateDrops[itemIndex].count, GameMaster.instance.crateDrops[itemIndex].durability);


        //drop = GameMaster.instance.crateDrops[itemIndex];
    }

    private void Update() {
        //Collider2D collider = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.NameToLayer("Player"));
        //print(collider);
        //if (collider != null && collider.gameObject == GameMaster.instance.player && Input.GetKeyDown(KeyCode.E)) {
        //    OnDeath();
        //}
        if (Input.GetKeyUp(Keys.openCrate)) {
            heldDuration = 0f;
        }

        if (getRemoved) {
            RemoveCrate();
        }
    }

    void RemoveCrate() {
        Destroy(gameObject);
        GameMaster.instance.gameStats.cratesBorken++;
        tilemap.SetTile(tilemap.WorldToCell(transform.position), null);
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject != null && collision.gameObject != GameMaster.instance.player) {
            return;
        }
        if (getRemoved) {
            return;
        }

        JPlayerController player = GameMaster.instance.player.GetComponent<JPlayerController>();
        if (player.holding)
            return;

        if (Input.GetKey(Keys.openCrate)) {
            heldDuration += Time.deltaTime;
        }

        if (collision.gameObject == GameMaster.instance.player && Input.GetKeyUp(Keys.openCrate)) {
            if (heldDuration < 0.5f) {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
                foreach (Collider2D hit in hits) {
                    if (hit.tag == "Player") {
                        continue;
                    }
                    JHealthController health = hit.gameObject.GetComponent<JHealthController>();
                    if (health != null) {
                        health.TakeDamage(5);
                    }
                }
                DroppedItem item = Instantiate(droppedItem, transform.position, Quaternion.identity) as DroppedItem;
                item.itemStack = drop;
                OnDeath();
            }

        }
        if (heldDuration > 0.5f) {
            heldDuration = 0f;
            GameObject clone = Instantiate(crateProjectile, Vector3.zero, Quaternion.identity);
            clone.transform.parent = GameMaster.instance.player.transform.GetChild(2);
            clone.transform.localPosition = new Vector3(0f, 0f, 0f);
            clone.GetComponent<CrateProjectile>().drop = drop;
            player.holding = true;
            player.inventory.Deselect();

            getRemoved = true;
        }
    }
}
