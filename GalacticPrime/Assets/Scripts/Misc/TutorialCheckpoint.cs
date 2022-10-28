using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCheckpoint : MonoBehaviour
{

    public int stage = 1;
    public bool playDialogueAtCheckpoint = true;

    private DialogueTrigger trigger;
    private Tutorial tutorial;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
        tutorial = transform.parent.GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial.tutorialStage == stage - 1 && transform.parent.GetChild(stage - 1).childCount == 0) {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag == "Player" && transform.parent.GetChild(stage - 1).childCount == 0) {
            if (tutorial.tutorialStage == stage - 1) {
                tutorial.tutorialStage = stage;
                if (playDialogueAtCheckpoint)
                    trigger.TriggerDialogue();
            }
            else if (tutorial.tutorialStage != stage) {
                //GameMaster.instance.player.GetComponent<JHealthController>().TakeDamage(99);
            }
        }
    }
}
