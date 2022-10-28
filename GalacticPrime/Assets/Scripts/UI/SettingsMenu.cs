using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer musicMixer;
    public GameObject credits;
    private bool settingKeybind = false;

    void Start() {
        if (LevelManager.instance.world == -2) {
            ShowCredits();
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (credits.activeSelf) {
                CloseCredits();
            }
            else {
                BackButton();
            }
        }
    }

    public void SetMusicVolume(float volume) {
        musicMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume) {
        musicMixer.SetFloat("SFXVolume", volume);
    }

    public void SetFullscreen(bool fullscrren) {
        Screen.fullScreen = fullscrren;
    }

    public void BackButton() {
        gameObject.SetActive(false);
    }

    public void ShowCredits() {
        credits.SetActive(true);
    }

    public void CloseCredits() {
        credits.SetActive(false);
    }
}
