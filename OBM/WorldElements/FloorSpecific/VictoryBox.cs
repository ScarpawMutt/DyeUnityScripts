/* Charlie Dye, PACE Team - 2026.03.27

This is the script for the trigger box that segues into the game over screen */

using UnityEngine;

public class VictoryBox : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the game over screens.")] public LossTransition ltReference;

    [Header("Object")]
    [Tooltip("The player's head.")] public GameObject cameraObject;

    [Header("Rigidbody Variable")]
    [Tooltip("The rigid body component attached to the poster concealing the secret text.")] public Rigidbody posterBody;

    [Header("Float Variable")]
    [Tooltip("The time between the initial trigger and the appearance of the game over screen.")] public float epilogueTimer;

    // Boolean variable
    private bool epilogueCanStart = false;

    void Start()
    {

        // If the float has an unworkable value, then this will correct it
        if (epilogueTimer < 0f) epilogueTimer = 0f;

    }

    void FixedUpdate()
    {

        // If the epilogue sequence is allowed to begin
        if (epilogueCanStart)
        {

            /* If the timer has time loaded onto it, then it will subtract unscaled time;
            otherwise, the transition to the game over screen will begin */
            if (epilogueTimer > 0f) epilogueTimer -= Time.fixedDeltaTime;
            else ltReference.playerHasWon = true;

        }

    }

    void OnTriggerEnter(Collider player)
    {
        
        // If the colliding object is the player with a collider equipped
        if (player == cameraObject.GetComponent<Collider>())
        {

            // If the poster's rigid body is ignoring gravity, then this will switch it
            if (!posterBody.useGravity) posterBody.useGravity = true;

            // Enables the epilogue proper to begin
            epilogueCanStart = true;

        }

    }

}
