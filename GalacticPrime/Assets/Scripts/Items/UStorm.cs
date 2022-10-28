using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UStorm : MonoBehaviour, IItem
{

    PointEffector2D effector;

    void Start()
    {
        effector = GetComponent<PointEffector2D>();
    }

    void Update()
    {
        
    }

    public float GetCooldown() {
        return 0f;
    }

    public void Use() {
        transform.SetParent(null);
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(UseStorm());
    }

    IEnumerator UseStorm() {
        GameMaster.instance.inventory.UseReusableItem();
        effector.enabled = true;
        yield return new WaitForSeconds(0.1f);
        effector.enabled = false;
        Destroy(gameObject);
    }
}
