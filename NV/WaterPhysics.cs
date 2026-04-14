/* Charlie Dye - 2025.03.25

This is the script for water physics */

using Oculus.Interaction.Locomotion;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{

    [Header("Script Reference")]
    public FirstPersonLocomotor fpsReference;

    [Header("Water Vision GameObject")]
    public GameObject planeOfObstruction;

    [Header("Player Controller")]
    public Collider playerWaterDetector;

    [Header("Modified Player Float Variables")]
    public float submergedSpeed;
    public float submergedJumpHeight;
    public float submergedGravity;
    private float originalSpeed;
    private float originalJumpForce;
    private float originalGravity;

    [Header("Boolean Variable")]
    public bool isSubmerged;

    void Start()
    {

        // Sets the private floats to match what was initially entered in the locomotor script
        originalSpeed = fpsReference.Acceleration;
        originalJumpForce = fpsReference.JumpForce;
        originalGravity = fpsReference.GravityFactor;

        // Initally disables the water effect
        planeOfObstruction.SetActive(false);

    }

    void FixedUpdate()
    {
        
        // Modifies player attributes depending on if the avatar is in water or not
        if (isSubmerged)
        {

            fpsReference.Acceleration = submergedSpeed;
            fpsReference.JumpForce = submergedJumpHeight;
            fpsReference.GravityFactor = submergedGravity;

        }
        else
        {

            fpsReference.Acceleration = originalSpeed;
            fpsReference.JumpForce = originalJumpForce;
            fpsReference.GravityFactor = originalGravity;

        }

    }

    void OnTriggerEnter(Collider legLevel)
    {

        if (legLevel == playerWaterDetector.GetComponent<Collider>())
        {

            // Submerges the player
            isSubmerged = true;

            // Causes the water effect to appear
            planeOfObstruction.SetActive(true);

        }

    }

    void OnTriggerExit(Collider legLevel)
    {

        if (legLevel == playerWaterDetector.GetComponent<Collider>())
        {

            // Un-submerges the player
            isSubmerged = false;

            // Causes the water effect to disappear
            planeOfObstruction.SetActive(false);

        }

    }

}
