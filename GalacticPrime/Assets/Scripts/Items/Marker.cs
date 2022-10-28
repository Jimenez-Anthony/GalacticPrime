using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public int damage = 0;

    void Start() {
        damage = 0;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player" || damage == 0) {
            return;
        }
        JHealthController health = collision.GetComponent<JHealthController>();
        if (health != null) {
            health.TakeDamage(damage);
        }
        JStatusController status = collision.GetComponent<JStatusController>();
        if (status != null) {
            status.ApplyStun(2f);
        }
    }
}
