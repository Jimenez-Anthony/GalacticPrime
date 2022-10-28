using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public float playerMaxHealth;
    public float playerCurrentHealth;
    public Image healthImage;
    public GameObject player;

    public AudioClip hurtSound;
    private AudioSource source;


    // Use this for initialization
    void Start () {
        playerCurrentHealth = playerMaxHealth;
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        healthImage.fillAmount = (playerCurrentHealth/100);


        if(playerCurrentHealth <= 0){
            Destroy(gameObject, 1f);
        }

        if (playerCurrentHealth >= 50)
        {
            healthImage.color = new Color32(0,137,0,255);
        }

        if (playerCurrentHealth < 50)
        {
            healthImage.color = new Color32(255, 239, 0, 255);
        }

        if (playerCurrentHealth <= 10)
        {
            healthImage.color = new Color32(255, 0, 0, 255);
        }
    }

    public void DamagePlayer(int damageToGive){
        playerCurrentHealth -= damageToGive;
        //source.PlayOneShot(hurtSound);
        AudioManager.instance.Play("HeartBeat");
    }

    public void SetMaxHealth(){
        playerCurrentHealth = playerMaxHealth;
    }


}
