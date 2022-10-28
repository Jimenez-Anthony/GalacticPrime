using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationTile : MonoBehaviour
{

    public GameObject station;

    void Start()
    {
        Instantiate(station, new Vector3(transform.position.x, transform.position.y + 1.5f, 0f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
