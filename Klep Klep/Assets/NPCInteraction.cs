using System.Collections;
using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public float interactionDistance = 3f; // Distance within which player can interact
    public float walkAwayDistance = 5f; // Distance at which NPC will disappear
    public float moveSpeed = 2f; // Speed at which NPC walks away
    public float rotationSpeed = 5f; // Speed at which NPC rotates towards the walking direction
    public TextMeshProUGUI textComponent; // UI Text component for dialogue
    public string[] lines; // Lines of dialogue
    public float textSpeed; // Speed at which text appears
    public GameObject dialoguePanel; // Dialogue panel to show/hide
    public Animator animator; // Animator component for triggering animations

    private bool interacted = false; // Flag to check if player has interacted
    private Vector3 startPosition; // NPC's starting position
    private Vector3 walkDirection; // Direction in which NPC walks away
    private bool walkingAway = false; // Flag to start walking away
    private bool dialogueFinished = false; // Flag to check if dialogue is finished
    private int index; // Current line index
    private Transform playerTransform; // Player's transform
    private Vector3 lastPlayerPosition; // Last known position of the player

    void Start()
    {
        // Record the starting position of the NPC
        startPosition = transform.position;

        // Initialize dialogue
        textComponent.text = string.Empty;
        dialoguePanel.SetActive(false);

        // Assuming the player has a tag "Player"
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerPosition = playerTransform.position;
    }

    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        // Check if the player is within interaction distance
        if (playerDistance <= interactionDistance && !interacted)
        {
            // If the player clicks the mouse button and is near enough, start interaction
            if (Input.GetMouseButtonDown(0))
            {
                Interact();
            }
        }

        // Only move the NPC and trigger the walking animation after the dialogue is finished
        if (walkingAway && dialogueFinished)
        {
            // Rotate the NPC towards the walk direction smoothly
            Quaternion targetRotation = Quaternion.LookRotation(walkDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the NPC in the walkDirection
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            // Ensure walking animation is active
            animator.SetBool("IsWalking", true);

            // Check the distance from the starting position
            if (Vector3.Distance(startPosition, transform.position) >= walkAwayDistance)
            {
                // Stop the walking animation
                animator.SetBool("IsWalking", false);
                // Destroy the NPC after it reaches the walkAwayDistance
                Destroy(gameObject);
            }
        }

        // Check for player input to advance the dialogue
        if (Input.GetMouseButtonDown(0) && dialoguePanel.activeInHierarchy)
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

    // Call this method when the player interacts with the NPC
    public void Interact()
    {
        if (!interacted)
        {
            interacted = true;
            StartDialogue();
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
            OnDialogueFinished();
        }
    }

    private void OnDialogueFinished()
    {
        dialogueFinished = true;

        if (dialogueFinished)
        {
            // Define the direction for the NPC to walk away
            walkDirection = (new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))).normalized;
            walkingAway = true;

            // Trigger the walking animation
            animator.SetBool("IsWalking", true);
        }
    }
}
