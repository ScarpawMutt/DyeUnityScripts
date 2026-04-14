/* Charlie Dye, PACE Team - 2025.02.14

This is the script for the collision effect of projectiles */

using System.Collections;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to this object that contains the grounding data.")] public GroundedCheck gcReference;

    [Header("Collider GameObject")]
    [Tooltip("The object that the collider represents.")] public GameObject sprayForcefield;

    [Header("Time to Appear/Disappear")]
    [Tooltip("The time, in seconds, that it takes between particles starting/stopping and collision activating/deactivating.")] public float colliderLagTime;

    // Boolean variables
    public bool trueBoolDetected = false;
    public bool falseBoolDetected = false;

    void Start()
    {

        // If the lag time is an unusable value, this will correct it
        if (colliderLagTime <= 0f) colliderLagTime = 0.5f;

        if (trueBoolDetected) trueBoolDetected = false;
        if (!falseBoolDetected) falseBoolDetected = true;

    }

    void FixedUpdate()
    {

        // If the player is on or arriving at the fifth floor
        if (ebReference.arrayIndexer == 5)
        {

            // If the prop changes its grounded status, then the private bools will update
            if (gcReference.isGrounded && !trueBoolDetected)
            {

                // Calls the method
                Invoke(nameof(ManipulateCollider), colliderLagTime);

                // Private "kill booleans" prevent excessive calls of the code block
                trueBoolDetected = true;
                falseBoolDetected = false;

            }
            else if (!gcReference.isGrounded && !falseBoolDetected)
            {

                // Calls the method
                Invoke(nameof(ManipulateCollider), colliderLagTime);

                // Private "kill booleans" prevent excessive calls of the code block
                trueBoolDetected = false;
                falseBoolDetected = true;

            }

        }

    }

    public void ManipulateCollider()
    {

        /*If the prop is grounded, then the collider's gameObject will deactivate;
        otherwise, it will become active and allow for collisions */
        if (gcReference.isGrounded) sprayForcefield.SetActive(false);
        else sprayForcefield.SetActive(true);

    }

}
