using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFissure : MonoBehaviour, IItem {

    public float markerLife = 7f;
    public FissureFinder fissureFinder;

    private JPlayerController playerCont;
    private int dir = 0;

    void Start()
    {
        playerCont = GameMaster.instance.player.GetComponent<JPlayerController>();
    }

    void Update()
    {
        if (playerCont.facingRight) {
            dir = 1;
        }
        else {
            dir = -1;
        }

    }

    public float GetCooldown() {
        return 0f;
    }

    public void Use() {
        Destroy(gameObject);
        GameMaster.instance.inventory.UseReusableItem();
        FissureFinder finder = Instantiate(fissureFinder, transform.position, Quaternion.identity) as FissureFinder;
        finder.dir = dir;
        //RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y - 1.7f), new Vector2(transform.position.x + range * dir, transform.position.y));
    }
}
