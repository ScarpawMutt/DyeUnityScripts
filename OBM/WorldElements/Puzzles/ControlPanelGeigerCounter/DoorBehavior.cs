/* Charlie Dye - 2024.11.18

This is the script for the door mechanism */

using UnityEngine;

public class DoorBehavior : MonoBehaviour
{

    [Header("Door Object")]
    [Tooltip("The axis to rotate the door around.")] public Transform doorHinge;

    [Header("Particle System")]
    [Tooltip("The dust particle system that is attached to the door.")] public ParticleSystem dustCloud;

    [Header("Float Variables")]
    [Tooltip("The rate at which the door opens.")] public float doorSpeed;
    [Tooltip("The maximum allowed angle that the door should open to.")] public float openedAngle;
    [Tooltip("The duration of the dust animation.")] public float dustDuration;

    [Header("Boolean Variable")]
    [Tooltip("Should the door animation begin?")] public bool executeAnimation;
    private bool animationHasExecuted;

    void Start()
    {

        // If the float and boolvariables are configured improperly, then this will correct them
        if (doorSpeed <= 0f) doorSpeed = 1f;
        if (openedAngle <= 180f) openedAngle = 270f;
        if (dustDuration <= 0f) dustDuration = 1f;
        if (executeAnimation) executeAnimation = false;
        if (animationHasExecuted) animationHasExecuted = false;



    }

    void FixedUpdate()
    {

        // If the door has not yet been opened and the animation bool becomes true
        if (executeAnimation && !animationHasExecuted)
        {

            // Opens the door via the method
            OpenDoor();

        }

    }

    public void OpenDoor()
    {

        // Rotates the door object around its hinge if it has not reached the specified angle or its local Euler angle is 0
        if (doorHinge.transform.localRotation.eulerAngles.z == 0f || doorHinge.transform.localRotation.eulerAngles.z > openedAngle)
            doorHinge.transform.Rotate(doorSpeed * Time.deltaTime * Vector3.back, Space.Self);
        else
        {

            // Stops the animation via the kill bool
            animationHasExecuted = true;

            // Kicks up the dust on the wall
            KickUpDust();

        }

    }

    public void KickUpDust()
    {

        // Starts the particle system
        dustCloud.Play();

        // Stops the particle system after a given time
        Invoke(nameof(StopDust), dustDuration);

    }

    public void StopDust()
    {

        // Stops the particle system
        dustCloud.Stop();

    }

}
