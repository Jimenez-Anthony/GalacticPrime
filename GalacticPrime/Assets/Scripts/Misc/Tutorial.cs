using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    private JPlayerController playerCont;
    private Transform player;

    public Dialogue[] dialogues;
    public int tutorialStage = 0;

    // Start is called before the first frame update
    void Start() {
        player = GameMaster.instance.player.transform;
        playerCont = player.GetComponent<JPlayerController>();
        GameMaster.instance.playerRespawnPoint = new Vector3(2.45f, 16.39f, 0f);
        playerCont.GetComponent<Dash>().enabled = false;

        AudioManager.instance.Play("Tutorial");
    }

    // Update is called once per frame
    void Update() {
        if (player.GetComponent<JHealthController>().hp <= 0 && player.GetComponent<JHealthController>().hp > -9999) {
            player.GetComponent<JHealthController>().hp = -9999;
            transform.GetChild(0).GetComponent<DialogueTrigger>().TriggerDialogue();
            GameMaster.instance.playerRespawnPoint = transform.GetChild(tutorialStage).position;
            tutorialStage--;
        }

        //GameMaster.instance.playerRespawnPoint = transform.GetChild(tutorialStage).position;

        if (tutorialStage == 3) {
            playerCont.GetComponent<Dash>().enabled = true;
        }
        if (tutorialStage == 5) {
            if (transform.GetChild(5).childCount != 0) {
                transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
            }
            else {
                if (transform.GetChild(5).GetComponent<DialogueTrigger>() != null) {
                    transform.GetChild(5).GetComponent<DialogueTrigger>().TriggerDialogue();
                    Destroy(transform.GetChild(5).GetComponent<DialogueTrigger>());
                }

            }
        }
        if (tutorialStage == 6 && transform.GetChild(5).childCount != 0) {
            player.GetComponent<JHealthController>().TakeDamage(99);
            tutorialStage = 5;
        }

        if (tutorialStage == 7) {
            if (GameMaster.instance.gameStats.cratesBorken > 0) {
                if (transform.GetChild(7).GetComponent<DialogueTrigger>() != null) {
                    transform.GetChild(7).GetComponent<DialogueTrigger>().TriggerDialogue();
                    Destroy(transform.GetChild(7).GetComponent<DialogueTrigger>());
                }
            }
        }

        if (tutorialStage == 9) {
            if (transform.GetChild(9).childCount != 0) {
                transform.GetChild(9).GetChild(0).gameObject.SetActive(true);
            }
            else {
                if (transform.GetChild(9).GetComponent<DialogueTrigger>() != null) {
                    transform.GetChild(9).GetComponent<DialogueTrigger>().TriggerDialogue();
                    Destroy(transform.GetChild(9).GetComponent<DialogueTrigger>());
                }

            }
        }
        if (tutorialStage == 10 && transform.GetChild(9).childCount != 0) {
            player.GetComponent<JHealthController>().TakeDamage(99);
            tutorialStage = 9;
        }
        if (tutorialStage == 10) {
            if (transform.GetChild(10).GetChild(0).gameObject.active == false) {
                transform.GetChild(10).GetChild(0).gameObject.SetActive(true);
            }
        }


        if (tutorialStage == 11) {
            if (transform.GetChild(11).childCount != 0) {
                for (int i = 0; i < transform.GetChild(11).childCount; i++) {
                    transform.GetChild(11).GetChild(i).gameObject.SetActive(true);
                }
            }
            else {
                if (transform.GetChild(11).GetComponent<DialogueTrigger>() != null) {
                    transform.GetChild(11).GetComponent<DialogueTrigger>().TriggerDialogue();
                    Destroy(transform.GetChild(11).GetComponent<DialogueTrigger>());
                }

            }
        }
        if (tutorialStage == 12 && transform.GetChild(11).childCount != 0) {
            player.GetComponent<JHealthController>().TakeDamage(99);
            tutorialStage = 11;
        }
        if (tutorialStage == 12) {
            if (transform.GetChild(12).GetChild(0).gameObject.active == false) {
                transform.GetChild(12).GetChild(0).gameObject.SetActive(true);
            }
        }

        if (tutorialStage == 13) {
            if (transform.GetChild(13).childCount != 0) {
                for (int i = 0; i < transform.GetChild(13).childCount; i++) {
                    transform.GetChild(13).GetChild(i).gameObject.SetActive(true);
                }
            }
            else {
                if (transform.GetChild(13).GetComponent<DialogueTrigger>() != null) {
                    transform.GetChild(13).GetComponent<DialogueTrigger>().TriggerDialogue();
                    Destroy(transform.GetChild(13).GetComponent<DialogueTrigger>());
                }

            }
        }
        if (tutorialStage == 14 && transform.GetChild(13).childCount != 0) {
            player.GetComponent<JHealthController>().TakeDamage(99);
            tutorialStage = 13;
        }
    }

}
