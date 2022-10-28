using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{

    // SURFACE URL: http://dreamlo.com/lb/1fBMVo3VF0enQP31tuvqzw0h1J5ZAD_0649YyvAqgByQ
    // CAVE URL: http://dreamlo.com/lb/xZVg2EAGJ0q5aVxc6qub0wRp8OOVfO2kijTF5WVcB13g
    // SEWER URL: 

    public int selectedBoard;
    public Transform selectors;

    public Transform entries;
    public Transform entry;

    void Start()
    {
        SelectBoard(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Close();
        }
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void SelectBoard(int board) {
        selectedBoard = board;
        for (int i = 0; i < selectors.childCount; i++) {
            if (i == board) {
                //selectors.GetChild(i).gameObject.SetActive(true);
                selectors.GetChild(i).GetComponent<Button>().interactable = false;
            }
            else {
                //selectors.GetChild(i).gameObject.SetActive(false);
                selectors.GetChild(i).GetComponent<Button>().interactable = true;
            }
        }

        for (int i = 0; i < entries.childCount; i++) {
            Destroy(entries.GetChild(i).gameObject);
        }
        StopAllCoroutines();

        if (selectedBoard == 1) {
            print("[Leaderboard] Loading scores for surface level");
            dreamloLeaderBoard.GetSceneDreamloLeaderboard().SelectServer(1);
            StartCoroutine(LoadLeaderboard());
        }
        else if (selectedBoard == 2) {
            print("[Leaderboard] Loading scores for cave level");
            dreamloLeaderBoard.GetSceneDreamloLeaderboard().SelectServer(2);
            StartCoroutine(LoadLeaderboard());
        }
    }

    IEnumerator LoadLeaderboard() {
        dreamloLeaderBoard dreamlo = dreamloLeaderBoard.GetSceneDreamloLeaderboard();
        dreamlo.LoadScores();

        while (dreamlo.ToScoreArray() == null) {
            yield return null;
        }

        int count = 0;
        foreach (dreamloLeaderBoard.Score score in dreamlo.ToListHighToLow()) {
            if (count > 10) {
                break;
            }
            Transform clone = Instantiate(entry, entries, false) as Transform;
            clone.GetChild(0).GetComponent<TMP_Text>().text = FormatName(score.playerName);
            clone.GetChild(1).GetComponent<TMP_Text>().text = score.score.ToString();
            clone.GetChild(2).GetComponent<TMP_Text>().text = FormatTime(score.seconds / 100f);
            count++;
        }
        print("[Leaderboard] Finished loading scores for surface level");
    }

    string FormatName(string name) {
        return name.Replace("+", " ");
    }

    string FormatTime(float time) {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }
}
