using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageScript : MonoBehaviour {

    public AudioClip openSound;
    public Animator garageanim;
    BoxCollider2D garageCollider;
    private AudioSource source;

    private PlayerController player;

	// Use this for initialization
	void Start () {
        garageanim.SetBool("IsOpen", false);
        garageCollider = GetComponent<BoxCollider2D>();
        garageCollider.isTrigger = false;
        source = GetComponent<AudioSource>();
        player = GameObject.FindObjectOfType<PlayerController>();
	}

    // Update is called once per frame
    /*void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            garageanim.SetBool("IsOpen", true);
            garageCollider.isTrigger = true;
        }*/

    // JIMMY - Doors now detect when the player is within a certain distance
    void Update() {
        if (player.gameStage >= 1 && Vector3.Distance(player.transform.position, transform.position) < 2.85f) {
            StartCoroutine("Opening");
        }
    }

    //private void OnCollisionEnter2D(Collision2D opendoor)
    //{
    //    if (opendoor.gameObject.name == "Player")  // player touching door to open it
    //    {
    //        StartCoroutine("Opening");
    //    }
    //}
    IEnumerator Opening()
    {
        garageanim.SetBool("IsOpen", true);
        source.PlayOneShot(openSound);
        yield return new WaitForSeconds(1);  // opening garage
        Destroy(gameObject);
    }
}
