using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpikeScript : MonoBehaviour
{
    public PlayerHealth playerCurrentHealth;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D spiked)
    {
        if (spiked.gameObject.name == "Player")
        {
            Debug.Log("Dead");
            playerCurrentHealth.playerCurrentHealth = 0;
            //Destroy(spiked.gameObject);
        }
    }
}
