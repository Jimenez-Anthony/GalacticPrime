using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    private Image image;
    private TMP_Text amountText;
    JEnergyController energyCont;

    // Start is called before the first frame update
    void Start()
    {
        energyCont = GameMaster.instance.playerEnergy;
        amountText = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float ammoPercent = 1.0f * energyCont.energy / energyCont.maxEnergy;
        image.fillAmount = ammoPercent;
        amountText.text = energyCont.energy.ToString();
        if (ammoPercent <= 0f) {
            image.fillAmount = 0f;
            amountText.text = 0.ToString();
        }
    }
}
