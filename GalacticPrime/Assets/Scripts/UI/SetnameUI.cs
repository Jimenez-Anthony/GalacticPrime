using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetnameUI : MonoBehaviour
{

    public GameObject usernameField;
    private bool enteringUsername;
    private string nameEntered;

    void Start()
    {
        enteringUsername = false; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            ContinueToGame();
        }
    }

    public void SetName(string name) {
        nameEntered = name;
    }

    public void ContinueToGame() {
        if (nameEntered != "") {
            PlayerPrefs.SetString("playerName", nameEntered);
            print("[Main Menu] new player name: " + nameEntered);
            FindObjectOfType<MainMenu>().usernameField.GetComponent<InputField>().text = PlayerPrefs.GetString("playerName");
            gameObject.SetActive(false);
        }
    }
}
