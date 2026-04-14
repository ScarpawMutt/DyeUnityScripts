/* Charlie Dye, PACE Team - 2024.10.11

This is the script to begin the experience */

using System.Collections;
using UnityEngine;

public class GroundFloor : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the door button in the cubicle.")] public OpeningButton obReference;

    [Header("GameObjects")]
    [Tooltip("The text on the player's UI.")] public GameObject helmetText;
    [Tooltip("The door that opens to the elevator.")] public GameObject slidingDoor;

    [Header("Particle Effect")]
    [Tooltip("The dust effect that plays when the door is opening.")] public ParticleSystem doorDust;

    [Header("Float Variables")]
    [Tooltip("The distance that the door moves when opening.")] public float distanceDoorTravels;
    [Tooltip("The speed that the door moves when opening.")] public float doorSpeed;
    [Tooltip("The rate, in seconds, at which the coroutine refreshes.")] public float refreshRate;
    private Vector3 doorTerminus;

    // Boolean variable
    [HideInInspector] public bool doorCanOpen;

    void Start()
    {

        // Establishes the location for the door to stop at once it begins opening
        doorTerminus = new(slidingDoor.transform.position.x - distanceDoorTravels,
            slidingDoor.transform.position.y, slidingDoor.transform.position.z);

        // If the dust effect is active, this will disable it temporarily
        if (doorDust.emission.enabled) doorDust.Stop();

        // If the coroutine refresh rate is equal to or below zero, this will set it to a positive value
        if (refreshRate <= 0f) refreshRate = 0.01f;

        // Disables UI temporarily
        helmetText.SetActive(false);
        
    }

    void FixedUpdate()
    {
        
        if (obReference.buttonHasActivated && !doorCanOpen)
        {

            // Allows the player to move on to the first floor
            ebReference.floorHasBeenCompleted = true;

            // Starts the door motion coroutine
            StartCoroutine(OpenDoor());

            // Sets the bool to true to prevent multiple activations
            doorCanOpen = true;

        }

    }

    public IEnumerator OpenDoor()
    {

        // Plays the dust particle effect
        doorDust.Play();

        // Moves the door along the wall in a straight path
        while (slidingDoor.gameObject != null && slidingDoor.transform.position != doorTerminus)
        {

            slidingDoor.transform.position = Vector3.MoveTowards(slidingDoor.transform.position, doorTerminus, doorSpeed);

            yield return new WaitForSecondsRealtime(refreshRate);

        }

        // Stops the dust particle effect
        doorDust.Stop();

        yield break;

    }

}
