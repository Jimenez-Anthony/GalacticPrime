using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{

    public float spawnCooldown;
    private float spawnTimer;

    public GameObject echo;

    void Start()
    {
        spawnTimer = spawnCooldown;
    }

    void Update()
    {
        if (spawnTimer > 0f) {
            spawnTimer -= Time.deltaTime;
        }
        else {
            spawnTimer = spawnCooldown;
            GameObject clone = Instantiate(echo, transform.position, Quaternion.identity) as GameObject;
            Destroy(clone, 1f);
        }
    }
}
