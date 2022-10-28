using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    void Start() {
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject == GameMaster.instance.player) {
            print("[Level Exit] Level Complete!");
            GameMaster.instance.GameWon();

            foreach (JHealthController health in FindObjectsOfType<JHealthController>()) {
                if (health.gameObject.tag != "Player") {
                    health.gameObject.SetActive(false);
                }
            }
        }
    }
}
