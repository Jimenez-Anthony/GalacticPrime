using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;


public class CreditScript : MonoBehaviour
{

    public float waitTime = 0f;

    IEnumerator Continue()
    {
        yield return new WaitForSeconds(35f);
        AudioManager.instance.StopSounds();
        SceneManager.LoadScene("Title Screen");
    }

    void Start()
    {
        AudioManager.instance.Play("Credits");
        //StartCoroutine(Continue());

        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) {
        //    AudioManager.instance.StopSounds();
        //    SceneManager.LoadScene("Title Screen");
        //}
    }

    void Update() {
        waitTime += Time.deltaTime;

        if (waitTime >= 1) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) {
                AudioManager.instance.StopSounds();
                SceneManager.LoadScene("Title Screen");
            }
            if (waitTime > 36f) {
                AudioManager.instance.StopSounds();
                SceneManager.LoadScene("Title Screen");
            }
        }


    }


}

