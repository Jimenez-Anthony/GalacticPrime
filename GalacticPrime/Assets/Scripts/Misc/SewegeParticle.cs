using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewegeParticle : MonoBehaviour
{

    private float resetCooldown = 2f;
    private float resetTimer;
    private Collider2D col;
    private Collider2D playerCol;

    void Start()
    {
        col = GetComponent<Collider2D>();
        resetTimer = 0f;
    }

    void Update()
    {
        if (resetTimer > 0f) {
            resetTimer -= Time.deltaTime;
        }
        else {
            resetTimer = resetCooldown;
            if (playerCol != null) {
                print("reseeting collision");
                Physics2D.IgnoreCollision(col, playerCol, false);
                playerCol = null;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            playerCol = collision.collider;
            Physics2D.IgnoreCollision(col, collision.collider);
            resetTimer = resetCooldown;
            JStatusController status = collision.gameObject.GetComponent<JStatusController>();
            if (status != null) {
                status.ApplySlow(1f, 0.05f);
                status.ApplyPoison(3, 1f, 1, true);
            }
        }
        //JHealthController health = collision.gameObject.GetComponent<JHealthController>();
        //if (health != null) {
        //    if (Random.Range(0f, 1f) < 0.5f) {
        //        health.TakeDamage(1);
        //    }
        //}
        //JStatusController status = collision.gameObject.GetComponent<JStatusController>();
        //if (status != null) {
        //    status.ApplySlow(0.5f, 0.05f);
        //}
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
    }
}
