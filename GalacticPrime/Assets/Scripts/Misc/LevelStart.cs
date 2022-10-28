using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        player = GameMaster.instance.player.transform;
        player.position = transform.position;
        player.GetComponent<JStatusController>().ApplyInvulnerability(3f);
    }

}
