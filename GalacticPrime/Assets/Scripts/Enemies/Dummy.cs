using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDeathController {

    private Vector3 location;

    private void Start() {
        location = transform.position;
    }

    public void OnDeath() {
        transform.position = new Vector3(0f, 0f);
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn() {
        yield return new WaitForSeconds(1f);
        JHealthController health = GetComponent<JHealthController>();
        health.hp = health.maxHP;
        transform.position = location;

    }
}
