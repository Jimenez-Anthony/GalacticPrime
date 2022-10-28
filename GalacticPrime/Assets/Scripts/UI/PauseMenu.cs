using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public bool IsPaused = false;

    public GameObject pauseMenuUI;

    void Start() {
        IsPaused = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (IsPaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
        if (Input.GetKeyDown(KeyCode.M) && IsPaused)
        {
            LoadMenu();
        }
        if (Input.GetKeyDown(KeyCode.R) && IsPaused) {
            RestartLevel();
        }
        //if (Input.GetKeyDown(KeyCode.Escape) && (IsPaused == true) && Input.GetKeyDown(KeyCode.Z))
        //{
        //    QuitGame();
        //}
    }

    public void Resume ()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        GameMaster.instance.gameState = GameMaster.GAMESTATE.ingame;

        // JIMMY - RESUME SOUNDS
        AudioManager.instance.ResumeSounds();
    }

    public void Pause ()
    {
        IsPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameMaster.instance.gameState = GameMaster.GAMESTATE.paused;

        // JIMMY - PAUSE SOUNDS
        AudioManager.instance.PauseSounds();
    }

    public void LoadMenu()
    {
        IsPaused = false;
        Time.timeScale = 1;
        LevelManager.instance.world = -1;
        DestroyImmediate(LevelManager.instance.gameObject);
        SceneManager.LoadScene("Title Screen");
    }

    public void RestartLevel() {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        IsPaused = false;
        GameMaster.instance.TryAgain();
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
