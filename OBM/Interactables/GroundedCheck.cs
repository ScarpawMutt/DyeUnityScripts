/* Charlie Dye, PACE Team - 2024.09.05

This is the script to check if an interactable is grounded */

using UnityEngine;

public class GroundedCheck : MonoBehaviour
{

    [Header("LayerMask")]
    [Tooltip("The layer mask that counts as suitable ground for the object.")] public LayerMask solidGround;

    [Header("Float Variables")]
    [Tooltip("The proximity to the floor, in meters, where the object counts as \"grounded\".")] public float groundingDistance;
    [Tooltip("The height of the object's collider.")] public float colliderHeight;

    [Header("Grounded Boolean")]
    [Tooltip("Whether or not the object is grounded.")] public bool isGrounded;

    void FixedUpdate()
    {

        // Draws a raycast downwards; the bool returns true when it hits an object with the "Ground" layer mask
        isGrounded = Physics.Raycast(transform.position, Vector3.down, colliderHeight * 0.5f + groundingDistance, solidGround);

    }

}