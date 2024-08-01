using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue1 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public GameObject dialoguePanel; // Reference to the dialogue UI panel

    private int index;
    private bool dialogueStarted = false;

    void Start()
    {
        textComponent.text = string.Empty;
        dialoguePanel.SetActive(false); // Hide the dialogue panel initially
    }

    void Update()
    {
        if (dialogueStarted && Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true); // Show the dialogue panel
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Type out each character one by one
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialoguePanel.SetActive(false); // Hide the dialogue panel
            dialogueStarted = false; // End the dialogue
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player") && !dialogueStarted)
        {
            dialogueStarted = true;
            StartDialogue();
        }
    }
}
