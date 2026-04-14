/* Charlie Dye, PACE Team - 2026.03.30

This is the script to delay dialogue, if necessary */

using UnityEngine;

public class DialogueDelay : MonoBehaviour
{

    // Script refernece
    private DialogueTrigger dtReference;

    void Start()
    {

        // Fetches the reference
        dtReference = gameObject.GetComponent<DialogueTrigger>();

        // If the above referenced script can be interacted with, then this will disable it for now
        if (dtReference.canInteract) dtReference.canInteract = false;

    }

    public void EnableTrigger()
    {

        // Enables the referenced interaction Boolean
        dtReference.canInteract = true;

    }

}
