using UnityEngine;
using TMPro;

public class TipUI : MonoBehaviour
{

    public string[] tips;
    private TMP_Text text;

    void Awake() {
        text = GetComponent<TMP_Text>();
    }

    void OnEnable() {
        System.Random rd = new System.Random();
        string tip = "Tip: " + tips[rd.Next(0, tips.Length)];
        text.text = tip;
    }
}
