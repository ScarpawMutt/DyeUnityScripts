/* Charlie Dye, PACE Team - 2026.02.26

This is the script for dialogue triggers across the play area */

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    [Header("Object Variable")]
    [Tooltip("The player's head.")] public GameObject passedHeadObject;

    [Header("Boolean Variable")]
    [Tooltip("Can the trigger be interacted with?")] public bool canInteract;
    private bool hasTriggered = false;

    // Script variable
    private DialogueControl dcReference;

    // Collider variable
    private Collider triggerCollider;

    void Awake()
    {

        // Fetches the script
        dcReference = FindFirstObjectByType<DialogueControl>();

        // Loads this object's own trigger into the private variable
        triggerCollider = gameObject.GetComponent<Collider>();

    }

    void Start()
    {

        // If the collider is not configured to act as a trigger, then this will correct that
        if (!triggerCollider.isTrigger) triggerCollider.isTrigger = true;

    }

    void OnTriggerStay(Collider playerHead)
    {

        // If the trigger can be interacted with
        if (canInteract && !hasTriggered)
        {

            // If the player hits the trigger to start dialogue
            if (playerHead == passedHeadObject.GetComponent<Collider>())
            {

                // Increments the trigger index value
                dcReference.triggerIndexer++;

                // Updates the method
                dcReference.UpdateTriggers(true);

                // Switches the kill Boolean
                hasTriggered = true;

            }

        }

    }

}
