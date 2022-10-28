using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamage : MonoBehaviour
{

    public float fallDuration;
    private Rigidbody2D rb2d;
    private JStatusController statusCont;
    private JPlayerController playerCont;

    // Start is called before the first frame update
    void Start()
    {
        fallDuration = 0f;
        rb2d = GetComponent<Rigidbody2D>();
        statusCont = GetComponent<JStatusController>();
        playerCont = GetComponent<JPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(rb2d.velocity.y);
        if (rb2d.velocity.y > -4.5) {
            if (fallDuration > 0.6f && !playerCont.isClimbing) {
                TakeFallDamage();
            }
            fallDuration = 0;
        }

        fallDuration += Time.deltaTime;
    }

    void TakeFallDamage() {
        rb2d.velocity = Vector3.zero;
        statusCont.ApplyStun(Mathf.Pow((1.6f * fallDuration), 2f));
    }
}
