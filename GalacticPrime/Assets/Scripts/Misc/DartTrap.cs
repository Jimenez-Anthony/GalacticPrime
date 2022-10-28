using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartTrap : MonoBehaviour
{

    public int dir = 0;
    public float range = 5f;
    public LayerMask hitLayers;

    private bool shot = false;

    public TrapArrow arrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!shot) {
            RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).position, new Vector2(dir, 0), range, hitLayers);
            if (hit) {
                //print("Shooting " + hit.transform.tag);
                TrapArrow clone = Instantiate(arrow, transform.GetChild(0).position, Quaternion.identity) as TrapArrow;
                Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                clone.faceDir = dir;
                shot = true;
            }
        }
    }
}
