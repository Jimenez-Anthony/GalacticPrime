using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JFlyerController : MonoBehaviour
{
    private Rigidbody2D rb2d;

    private bool facingRight = false;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Move(float moveX, float moveY, Transform target) {
        Debug.Log("move: " + moveX + ", " + moveY);
        Vector3 targetVelocity = new Vector2(moveX * 10f, moveY * 15f);
        rb2d.velocity = targetVelocity;

        if (moveX > 0 && !facingRight) {
            Flip();
        }
        else if (moveX < 0 && facingRight) {
            Flip();
        }
    }

    private void Flip() {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
