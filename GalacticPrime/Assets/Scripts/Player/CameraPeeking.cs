using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPeeking : MonoBehaviour
{
    float maxDistance = 1.3f;

    void Update() {
        if (transform.localPosition.y > 0 && !Input.GetKey(KeyCode.LeftControl)) {
            //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 10f * Time.deltaTime, transform.localPosition.z);
            //if (transform.localPosition.y - 0 < 0.1) {
                transform.localPosition = Vector3.zero;
            //}
        }

        if (transform.localPosition.y < 0 && !Input.GetKey(KeyCode.LeftControl)) {
            //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 10f * Time.deltaTime, transform.localPosition.z);
            //if (transform.localPosition.y - 0 > -0.1) {
                transform.localPosition = Vector3.zero;
            //}
        }

        if (transform.localPosition.y < maxDistance && (Input.GetKey(Keys.panCamera)) && Input.GetKey(KeyCode.W)) {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 50f * Time.deltaTime, transform.localPosition.z);
        }

        if (transform.localPosition.y > -maxDistance * 1.1 && (Input.GetKey(Keys.panCamera)) && Input.GetKey(KeyCode.S)) {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 50f * Time.deltaTime, transform.localPosition.z);
        }
    }
}
