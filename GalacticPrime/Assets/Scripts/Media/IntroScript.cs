using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System;

public class IntroScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float waitTime = 0f;
    void Update()
    {

        if (waitTime >= 2)
        {
            if (videoPlayer.isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                {
                    LevelManager.instance.LoadTutorial();
                }
            }
            else
            {
                LevelManager.instance.LoadTutorial();
            }
        }
        else
            waitTime += Time.deltaTime;
    }
}