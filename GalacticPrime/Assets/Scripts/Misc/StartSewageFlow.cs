using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSewageFlow : MonoBehaviour
{

    private Transform player;
    //private List<Transform> children;

    void Start()
    {
        player = GameMaster.instance.player.transform;
        //children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
            //children.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= 12f) {
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(true);
                //children.Add(transform.GetChild(i));
            }
            this.enabled = false;
        }
    }
}
