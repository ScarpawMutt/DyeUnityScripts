/* Charlie Dye, PACE Team - 2026.04.09

This is the script for the combination puzzle buttons */

using UnityEngine;

public class CombinationButton : MonoBehaviour
{

    [Header("Objects")]
    [Tooltip("The player's left hand collider.")] public GameObject handColliderLeft;
    [Tooltip("The player's right hand collider.")] public GameObject handColliderRight;

    [Header("Boolean Variable")]
    [Tooltip("Is the button currently being interacted with?")] public bool isActive;

    void OnTriggerEnter(Collider playerHands)
    {

        // If the player uses their hands to interact with this button's trigger collider, then the "active" Boolean will become true
        if (playerHands == handColliderLeft.GetComponent<Collider>() || playerHands == handColliderRight.GetComponent<Collider>()) isActive = true;

    }

    void OnTriggerExit(Collider playerHands)
    {

        // If the player uses their hands to stop interacting with this button's trigger collider, then the "active" Boolean will become false
        if (playerHands == handColliderLeft.GetComponent<Collider>() || playerHands == handColliderRight.GetComponent<Collider>()) isActive = false;

    }

}
