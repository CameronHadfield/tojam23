using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Dialogue Implementation -
Credits to Darren Tran
*/
public class DialogueManager : MonoBehaviour
{
    [Header("Object References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;
    public AudioSource playsound;

    private Queue<Sentence> sentences;

    public delegate void OnLastDialogue();
    public event OnLastDialogue LastDialogue;

    public delegate void OnGameOver();
    public event OnGameOver GameOver;

    // Check if the user was pressing skip from the previous Dialogue sentence.
    private bool skippingPrevious = false;

    private float nextDialogueTimer = 3f;

    public DialogueTrigger currentTrigger;

    public void TriggerDialogueObject(string dialogueObjectName)
    {
        GameObject go = GameObject.Find(dialogueObjectName);
        DialogueTrigger trigger = (DialogueTrigger)go.GetComponent(typeof(DialogueTrigger));
        trigger.TriggerDialogue();
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger trigger)
    {
        if (currentTrigger != null)
            DestroyTrigger();
        currentTrigger = trigger;

        // Triggers the ending of the game.
        if (dialogue.type == DialogueType.Ending)
        {
            LastDialogue?.Invoke();
        }

        animator.SetBool("isOpen", true);

        sentences = new Queue<Sentence>();
        sentences.Clear();

        foreach (Sentence sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence(dialogue);
    }

    public void DisplayNextSentence(Dialogue dialogue)
    {
        if (sentences.Count == 0)
        {
            EndDialogue(dialogue);
            return;
        }

        Sentence sentence = sentences.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, dialogue));
    }

    IEnumerator TypeSentence(Sentence sentence, Dialogue dialogue)
    {
        nameText.text = sentence.name;
        dialogueText.text = "";

        playsound.GetComponent<AudioSource>().Play();

        foreach (char letter in sentence.text.ToCharArray())
        {
            dialogueText.text += letter;
            if (!skippingPrevious && Input.GetButton("Skip"))
            {
                dialogueText.text = sentence.text;
                while (!Input.GetButtonUp("Skip"))
                {
                    yield return null;
                }
                break;
            }
            skippingPrevious = Input.GetButton("Skip");

            yield return new WaitForSeconds(0.02f);
        }

        skippingPrevious = true;
        nextDialogueTimer = 3f;

        while (!Input.GetButtonDown("Skip") && nextDialogueTimer > 0)
        {
            nextDialogueTimer -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.02f);
        DisplayNextSentence(dialogue);
    }

    void EndDialogue(Dialogue dialogue)
    {
        animator.SetBool("isOpen", false);
        DestroyTrigger();
        currentTrigger = null;

        // Game is over
        if (dialogue.type == DialogueType.Ending)
        {
            GameOver?.Invoke();
        }
    }

    void DestroyTrigger()
    {
        currentTrigger.TriggerDialogueFinish();
        Destroy(currentTrigger.gameObject);
    }
}
