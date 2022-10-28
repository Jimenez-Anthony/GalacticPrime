using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

    public float degrees = 360f;
    
    void Update () {
        transform.Rotate(new Vector3(0f, 0f, degrees * Time.deltaTime));
    }
}
