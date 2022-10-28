using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechMageAttackScript : MonoBehaviour {

    Rigidbody2D rb;
    Animator mechattackanim;
    BoxCollider2D laserCollider;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("Lasering");
	}


    IEnumerator Lasering () {
        yield return new WaitForSeconds(.8f);  // laser firing
        Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == ("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(10);  // damaging player
        }
    }

}

