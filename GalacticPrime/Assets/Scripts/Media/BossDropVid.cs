using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System;

public class BossDropVid : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject finalScreen;
    public float waitTime = 0f;

    void Update()
    {
        if (waitTime >= 1) {
            if (videoPlayer.isPlaying) {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) {
                    StartCoroutine(ShowEndScreen());
                }
            }
            else {
                StartCoroutine(ShowEndScreen());
            }
        }
        else
            waitTime += Time.deltaTime;

    }

    IEnumerator ShowEndScreen() {
        //finalScreen.SetActive(true);
        //yield return new WaitForSeconds(2f);
        //SceneManager.LoadScene("Title Screen");
        yield return null;
        LevelManager.instance.NextLevel();
    }
}