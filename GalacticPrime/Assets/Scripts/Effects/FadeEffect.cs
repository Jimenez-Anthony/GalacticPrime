using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{

    public byte speed = 5;

    private SpriteRenderer sr;
    private byte alpha = 255;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (speed <= alpha) {
            alpha -= speed;
        }
        sr.color = new Color32(255, 255, 255, alpha);

    }
}
