using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {


    public GameObject loadingScreen;
    public GameObject gamemodes;
    public GameObject settings;
    public GameObject usernameField;
    public GameObject setnameUI;
    public GameObject leaderboards;

    private bool enteringUsername;

    void Awake() {
        LoadKeybindsFromFile();
        //PlayerPrefs.DeleteAll();
    }

    void Start() {
        settings.GetComponent<SettingsMenu>().SetFullscreen(true);
        if (LevelManager.instance.world == -2) {
            ShowMenu();
        }
        AudioManager.instance.Play("TitleSong");

        enteringUsername = false;

        if (PlayerPrefs.HasKey("playerName")) {
            usernameField.GetComponent<InputField>().text = PlayerPrefs.GetString("playerName");
        }
        else {
            setnameUI.SetActive(true);
        }
    }

    void LoadKeybindsFromFile() {
        print("[Main Menu] Loading keybinds from file...");
        SetKeyToPlayerpref(Keys.left, "left");
        SetKeyToPlayerpref(Keys.right, "right");
        SetKeyToPlayerpref(Keys.useItem, "useItem");
        SetKeyToPlayerpref(Keys.dropItem, "dropItem");
        SetKeyToPlayerpref(Keys.restartLevel, "restartLevel");
        SetKeyToPlayerpref(Keys.openCrate, "openCrate");
        SetKeyToPlayerpref(Keys.nextItem, "nextItem");
        SetKeyToPlayerpref(Keys.previousItem, "previousItem");
        SetKeyToPlayerpref(Keys.dash, "dash");
        SetKeyToPlayerpref(Keys.ladderUp, "ladderUp");
        SetKeyToPlayerpref(Keys.ladderDown, "ladderDown");
        SetKeyToPlayerpref(Keys.itemPanel, "itemPanel");
        SetKeyToPlayerpref(Keys.panCamera, "panCamera");
        SetKeyToPlayerpref(Keys.advanceDialogue, "advanceDialogue");
        SetKeyToPlayerpref(Keys.jump, "jump");
    }

    void SetKeyToPlayerpref(KeyCode key, string name) {
        if (PlayerPrefs.HasKey(name)) {
            key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(name));
            typeof(Keys).GetField(name).SetValue(null, key);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.M) && !enteringUsername) {
            ShowMenu();
        }
        if (Input.GetKeyDown(KeyCode.Return) && !enteringUsername && !setnameUI.activeSelf) {
            ShowGameModes();
        }

        if (usernameField.GetComponent<InputField>().isFocused) {
            enteringUsername = true;
        }
        else {
            enteringUsername = false;
        }
    }

    public void ShowMenu() {
        settings.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ShowGameModes() {
        gamemodes.SetActive(true);
    }

    public void ShowLeaderboard() {
        leaderboards.SetActive(true);
    }

    public void ChangePlayerName(string name) {
        if (name != "") {
            if (PlayerPrefs.HasKey("playerName")) {
                PlayerPrefs.SetString("playerName", name);
                print("[Main Menu] Player name changed to " + name);
            }
            else {
                print("[Main Menu] Error: Player name save data does not exist");
            }
        }
        else {
            usernameField.GetComponent<InputField>().text = PlayerPrefs.GetString("playerName");
        }
    }

}
