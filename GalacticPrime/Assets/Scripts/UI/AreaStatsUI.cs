using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AreaStatsUI : MonoBehaviour
{
    private TMP_Text title;
    private TMP_Text score;
    private TMP_Text stats;

    void Awake() {
        title = transform.GetChild(0).GetComponent<TMP_Text>();
        score = transform.GetChild(1).GetComponent<TMP_Text>();
        stats = transform.GetChild(3).GetComponent<TMP_Text>();
    }

    void OnEnable() {
        if (LevelManager.instance.world == 1) {
            title.text = "Surface Area Complete!";
            dreamloLeaderBoard.GetSceneDreamloLeaderboard().SelectServer(1);
        }
        else if (LevelManager.instance.world == 2) {
            title.text = "Cave Area Complete!";
            dreamloLeaderBoard.GetSceneDreamloLeaderboard().SelectServer(2);
        }
        else if (LevelManager.instance.world == 3) {
            title.text = "Sewer Area Complete!";
        }

        score.text = "Score: " + GetScore().ToString();
        stats.text = CreateStatsText();
    }

    string CreateStatsText() {
        string output = "";
        output += "Area Time: " + FormatTime(LevelManager.instance.levelStats.timeTaken) + "\t";
        output += "Damage Dealt: " + LevelManager.instance.levelStats.damageDealt + "\n";
        output += "Damage Taken: " + LevelManager.instance.levelStats.damageTaken + "\t\t";
        output += "Amount Healed: " + LevelManager.instance.levelStats.amountHealed + "\n";
        output += "Crates Opened: " + LevelManager.instance.levelStats.cratesBorken + "\t\t";
        output += "Jumps: " + LevelManager.instance.levelStats.jumpTimes + "\n";
        output += "\t\tDeaths: " + LevelManager.instance.timesDied;
        return output;
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

    int GetScore() {
        float score = 10000f;
        score /= Mathf.Sqrt(LevelManager.instance.levelStats.timeTaken);
        score += (LevelManager.instance.levelStats.damageDealt * 10f);
        score -= LevelManager.instance.levelStats.damageTaken * 5f;
        score += LevelManager.instance.levelStats.amountHealed * 2.5f;
        score += LevelManager.instance.levelStats.cratesBorken * 10f;
        score += LevelManager.instance.levelStats.jumpTimes;
        for (int i = 0; i < LevelManager.instance.bossKills; i++) {
            score *= 1.5f;
        }
        for (int i = 0; i < LevelManager.instance.timesDied; i++) {
            score *= 0.8f;
        }
        UploadScore(LevelManager.instance.levelStats.timeTaken, (int)score);
        return (int)score;
    }

    void UploadScore(float time, int score) {
        dreamloLeaderBoard.GetSceneDreamloLeaderboard().AddScore(PlayerPrefs.GetString("playerName"), score, (int)(time * 100f + 0.5f));
        print("[Scoreboard] Uploaded new score for " + PlayerPrefs.GetString("playerName") + ": " + score + ", " + (int)time);
    }

    void Update()
    {
        
    }
}
