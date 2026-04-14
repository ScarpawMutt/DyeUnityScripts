/* Charlie Dye, PACE Team - 2026.02.09

This is the master script for the rolling credits */

using System.Collections;
using UnityEngine;

public class CreditBehavior : MonoBehaviour
{

    [Header("Game Objects")]
    [Tooltip("A single object containing all UI text elements for the credits as children.")] public GameObject creditTextBoxes;
    [Tooltip("The parent object of the dynamic text, with offset included.")] public GameObject masterTextObject;
    [Tooltip("The main camera object.")] public GameObject playerCamera;

    [Header("Numerical Variables")]
    [Tooltip("The Y-position that each text box starts at before moving across the screen.")] public float startingPosition;
    [Tooltip("The Y-position that each text box should end at once off the screen.")] public float endingPosition;
    [Tooltip("The speed at which the text moves across the screen.")] public float scrollSpeed;
    [Tooltip("The rotation speed of the text while following the user's gaze. This can be left at zero for no movement.")] public float gazeRate;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;

    // Boolean variable
    private bool canFollowGaze;

    void Start()
    {

        // If the text box object is left null, then this script will self-destruct
        if (creditTextBoxes == null || masterTextObject == null) Destroy(this);

        // If the float values have improper values, then this will correct them
        if (startingPosition > 0f) startingPosition = -800f;
        if (endingPosition < 0f) endingPosition = 800f;
        if (scrollSpeed == 0f) scrollSpeed = 1f;
        else if (scrollSpeed < 0f) scrollSpeed *= -1f;
        if (gazeRate < 0f) gazeRate *= -1f;

        // If the gaze rate is set to zero, then the following movement will switch off by way of the Boolean
        if (gazeRate == 0f) canFollowGaze = false;
        else canFollowGaze = true;

        // Moves the text object to its starting position on the canvas
        creditTextBoxes.transform.localPosition = new Vector3(0f, startingPosition, 0f);

        // Starts the coroutines
        StartCoroutine(MasterScroll());
        if (canFollowGaze) StartCoroutine(FollowCamera());
        
    }

    private IEnumerator MasterScroll()
    {

        while (creditTextBoxes != null)
        {

            if (creditTextBoxes.transform.localPosition.y < endingPosition)
            {

                // Moves the text across and up the screen, creating the illusion of scrolling
                creditTextBoxes.transform.localPosition = Vector3.MoveTowards(creditTextBoxes.transform.localPosition, new Vector3(0f, endingPosition, 0f), scrollSpeed);

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else
            {

                // Deactivates the text object
                creditTextBoxes.SetActive(false);

                // Breaks the coroutine
                yield break;

            }

        }

    }

    private IEnumerator FollowCamera()
    {

        // While-loop that returns true as long as the camera object and text are active
        while (playerCamera != null && masterTextObject != null)
        {

            // If the follow movement can still go on
            if (canFollowGaze)
            {

                Quaternion trimmedVectorCamera = new (0f, playerCamera.transform.rotation.y, 0f, playerCamera.transform.rotation.w);
                Quaternion trimmedVectorCredits = new (0f, masterTextObject.transform.rotation.y, 0f, masterTextObject.transform.rotation.w);

                // Spherically interpolates between the camera and credit rotations, creating a gentle follow effect to wherever the viewer looks on the Y-rotation
                masterTextObject.transform.rotation = Quaternion.Slerp(trimmedVectorCredits, trimmedVectorCamera, gazeRate * Time.fixedDeltaTime);

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            // if it must stop
            else yield break;

        }

    }

}
