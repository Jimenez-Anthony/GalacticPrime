using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{

    public ItemStack itemStack;
    private Bounds colliderBounds;

    public float pickUPTimer;

    void Start() {
        if (GetComponent<SpriteRenderer>() == null) {
            gameObject.AddComponent<SpriteRenderer>();
        }
        GetComponent<SpriteRenderer>().sprite = itemStack.item.icon;
        Vector2 s = GetComponent<SpriteRenderer>().sprite.bounds.size;
        if (GetComponent<BoxCollider2D>() == null) {
            gameObject.AddComponent<BoxCollider2D>().isTrigger = false;
        }
        GetComponent<BoxCollider2D>().size = new Vector3(1f, 1f, 1f);
        colliderBounds = GetComponent<SpriteRenderer>().sprite.bounds;
        //GetComponent<BoxCollider2D>().offset = new Vector2((s.x / 2), 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (pickUPTimer > 0f) {
            pickUPTimer -= Time.deltaTime;
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.35f);
        foreach (Collider2D col in colliders) {
            //print(col.name);
            if (col.tag == "Player") {
                //print("player found");
                Inventory inv = col.GetComponent<Inventory>();
                if (inv.PickUpItem(itemStack)) {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
