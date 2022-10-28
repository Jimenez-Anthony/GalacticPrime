using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SargeyDialogue : MonoBehaviour
    
    

{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Triggering");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    IEnumerator Triggering()
    {
        FindObjectOfType<DialogueTrigger>().TriggerDialogue();
        yield return new WaitForSeconds(4f);
        FindObjectOfType<DialogueTrigger>().TriggerDialogue();
        yield return new WaitForSeconds(4f);
        FindObjectOfType<DialogueTrigger>().TriggerDialogue();
        yield return new WaitForSeconds(4f);
        FindObjectOfType<DialogueManager>().DisplayNextSentence();
    }*/
}
