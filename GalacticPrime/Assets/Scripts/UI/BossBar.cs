using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBar : MonoBehaviour
{

    public string name;
    public float displayDistance = 20f;
    
    private JHealthController health;
    private GameObject bossPanel;
    private TMP_Text text;
    private Image image;

    void Start()
    {
        health = GetComponent<JHealthController>();
        bossPanel = GameMaster.instance.bossPanel;
        text = bossPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        image = bossPanel.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        if (Vector3.Distance(GameMaster.instance.player.transform.position, transform.position) <= displayDistance) {
            text.text = name;
            image.fillAmount = 1.0f * health.hp / health.maxHP;
            bossPanel.SetActive(true);
        }
        else {
            bossPanel.SetActive(false);
        }
    }

    void OnDestroy() {
        if (bossPanel != null) {
            bossPanel.SetActive(false);
        }
    }
}
