using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    public PlayerHealth playerCurrentHealth;
    public GameObject gameOverMenuUI;

    public static bool IsGameOver = false;

    // Update is called once per frame
    void Update()
    {
        if (playerCurrentHealth.playerCurrentHealth <= 0)
        {
            DeadMenu();
        }

        if (Input.GetKeyDown(KeyCode.R) && (IsGameOver == true))
        {
            Retry();
        }
        if (Input.GetKeyDown(KeyCode.M) && (IsGameOver == true))
        {
            LoadMenu();
        }
        if (Input.GetKeyDown(KeyCode.Q) && (IsGameOver == true) && Input.GetKeyDown(KeyCode.Z))
        {
            QuitGame();
        }
    }

    public void DeadMenu()
    {
        gameOverMenuUI.SetActive(true);
        Time.timeScale = 0;
        IsGameOver = true;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        IsGameOver = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title Screen");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
