using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    private float targetSpeed = 120f;

    private Transform player;
    private JPlayerController playerCont;
    public int playerDirection;
    private new SpriteRenderer renderer;
    private bool targeting = false;
    public bool goingDown = false;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent;
        renderer = GetComponent<SpriteRenderer>();
        playerCont = player.GetComponent<JPlayerController>();
        renderer.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCont.facingRight) {
            playerDirection = 1;
        }
        else {
            playerDirection = -1;
        }

        if (Mathf.Abs(transform.position.x - player.transform.position.x) < 0.05f) {
            if (!goingDown) {
                goingDown = true;
            }
            else {
                goingDown = false;
            }
        }

        if (targeting) {
            if (playerDirection == 1) {
                if (!goingDown) {
                    transform.RotateAround(player.transform.position, new Vector3(0f, 0f, 1f), Time.deltaTime * targetSpeed);
                }
                else {
                    //print("going down");
                    transform.RotateAround(player.transform.position, new Vector3(0f, 0f, -1f), Time.deltaTime * targetSpeed);
                }
            }
            else {
                if (!goingDown) {
                    transform.RotateAround(player.transform.position, new Vector3(0f, 0f, -1f), Time.deltaTime * targetSpeed);
                }
                else {
                    //print("going down");
                    transform.RotateAround(player.transform.position, new Vector3(0f, 0f, 1f), Time.deltaTime * targetSpeed);
                }
            }

        }
    }

    public void StartTargeting() {
        targeting = true;
        renderer.enabled = true;
        goingDown = false;
        transform.localPosition = new Vector3(0.6f, 0f, 0f);
    }

    public Vector3 GetTargetPosition() {
        renderer.enabled = false;
        return transform.position;
    }
}
