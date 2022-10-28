using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JDialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private bool displaying = false;
    private bool next = false;
    private Coroutine displayCo;

    //public TMP_Text nameText;
    public TMP_Text dialogueText;

    public Animator anim;
    public float delay = 0.2f;

    // JIMMY - SINGLEN PATTERN
    public static JDialogueManager instance;
    void Awake() {
        // Singleton Pattern
        if (instance != null) {
            return;
        }
        instance = this;

        sentences = new Queue<string>();
    }

    void Update() {
        if (Input.GetKeyDown(Keys.advanceDialogue)) {
            if (!displaying) {
                displayNextSentence();
            }
            else {
                next = true;
            }
        }
    }

    public void startDialogue(Dialogue dialogue) {
        Debug.Log("Starting dialogue with " + dialogue.name);
        sentences.Clear();
        anim.SetBool("isOpen", true);

        //Time.timeScale = 0;
        GameMaster.instance.player.GetComponent<JHealthController>().invulnerable = true;
        GameMaster.instance.player.GetComponent<JPlayerController>().stunned = true;
        GameMaster.instance.player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        foreach (string s in dialogue.sentences) {
            sentences.Enqueue(s);
        }

        displayNextSentence();
    }

    public void displayNextSentence() {
        if (sentences.Count == 0) {
            end();
            return;
        }

        string sentence = sentences.Dequeue();
        displayCo = StartCoroutine(display(sentence));
    }

    IEnumerator display(string s) {
        displaying = true;
        dialogueText.text = "";
        foreach (char letter in s.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
            if (next) {
                next = false;
                dialogueText.text = s;
                break;
            }
        }
        displaying = false;
    }

    void end() {
        anim.SetBool("isOpen", false);
        //Time.timeScale = 1;
        GameMaster.instance.player.GetComponent<JHealthController>().invulnerable = false;
        GameMaster.instance.player.GetComponent<JPlayerController>().stunned = false;
        GameMaster.instance.player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
}
