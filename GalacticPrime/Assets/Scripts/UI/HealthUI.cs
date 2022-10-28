using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    private Image image;
    JHealthController playerHealthCont;
    private float lastPercentage;
    private bool takingDamage;

    private Image amountImage;
    private TMP_Text amountText;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        amountImage = transform.GetChild(0).GetComponent<Image>();
        amountText = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        playerHealthCont = GameMaster.instance.player.GetComponent<JHealthController>();
        takingDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        float healthPercentage = 1.0f * playerHealthCont.hp / playerHealthCont.maxHP;
        image.fillAmount = healthPercentage;
        amountText.text = playerHealthCont.hp.ToString();
        if (playerHealthCont.hp <= 0) {
            image.fillAmount = 0f;
            amountText.text = 0.ToString();
        }

        if (!takingDamage) {
            if (playerHealthCont.hp <= 0) {
                //Destroy(gameObject, 1f);
            }

            if (healthPercentage >= .5) {
                image.color = new Color32(0, 137, 0, 255);
                amountImage.color = new Color32(0, 137, 0, 255);
            }

            if (healthPercentage < .5) {
                image.color = new Color32(255, 239, 0, 255);
                amountImage.color = new Color32(255, 239, 0, 255);
            }

            if (healthPercentage <= .1) {
                image.color = new Color32(255, 0, 0, 255);
                amountImage.color = new Color32(255, 0, 0, 255);
            }

            if (healthPercentage > 1f) {
                image.color = new Color32(0, 255, 0, 255);
                //amountImage.color = new Color32(0, 255, 0, 255);
            }
        }

        if (healthPercentage < lastPercentage) {
            StartCoroutine(TakeDamage());
        }

        lastPercentage = healthPercentage;
    }

    IEnumerator TakeDamage() {
        takingDamage = true;
        image.color = new Color32(255, 255, 255, 150);
        amountImage.color = new Color32(255, 255, 255, 150);
        yield return new WaitForSeconds(0.07f);
        takingDamage = false;

    }
}
