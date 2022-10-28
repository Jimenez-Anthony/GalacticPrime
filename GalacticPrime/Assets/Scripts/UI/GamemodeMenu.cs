using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeMenu : MonoBehaviour
{

    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            LevelManager.instance.StartGame();
        }
    }

    public void BackButton() {
        gameObject.SetActive(false);
    }
}
