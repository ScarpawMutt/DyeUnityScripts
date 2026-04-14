/* Charlie Dye - ECT 4440 - 2026.01.20

This is the script for controlling the spaceship */

using TMPro;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpaceshipBehavior : MonoBehaviour
{

    [Header("Script and Input Action References")]
    [Tooltip("The script attached to the camera that takes over while the player is downed.")] public DeathSequence dsReference;
    [Tooltip("The script attached to the master game controls.")] public GameControls gcReference;
    [Tooltip("The script attached to the level controls.")] public LevelCounter lcReference;
    [Tooltip("The script attached to the player's power-up counter.")] public PowerUpBank pubReference;
    [Tooltip("The script attached to the player's UI.")] public TMPBehavior tmpReference;

    [Header("Input Actions")]
    [Tooltip("The keyboard binding to move the ship forward.")] public InputAction maneuverUp;
    [Tooltip("The keyboard binding to move the ship backward.")] public InputAction maneuverDown;
    [Tooltip("The keyboard binding to move the ship to the left.")] public InputAction maneuverLeft;
    [Tooltip("The keyboard binding to move the ship to the right.")] public InputAction maneuverRight;
    [Tooltip("The keyboard binding to rotate the ship clockwise.")] public InputAction rotateClockwise;
    [Tooltip("The keyboard binding to rotate the ship counterclockwise.")] public InputAction rotateCounterclockwise;

    [Header("Object")]
    [Tooltip("A demarcating object separating the main arena from the danger zone.")] public GameObject borderMarker;

    [Header("Rigidbody")]
    [Tooltip("The rigid body of the ship decoration that the camera is attached to.")] public Rigidbody cockpitObject;

    [Header("Image")]
    [Tooltip("The red vignette that slowly fades in as the player goes out of bounds.")] public Image warningVignette;

    [Header("Float Variables")]
    [Tooltip("The speed of the spaceship relative to a given point.")] public float shipSpeed;
    [Tooltip("The speed at which the spaceship rotates on its axis.")] public float rotationSpeed;
    [Tooltip("The rate of linear damping if the player has the \"Clear Steer\" buff applied to the ship.")] public float straightLineDamping;
    [Tooltip("The rate of angular damping if the player has the \"Clear Steer\" buff applied to the ship.")] public float rotationalDamping;
    [Tooltip("The maximum allowed speed that the ship is able to travel relative to a fixed point.")] public float maximumLinearVelocity;
    [Tooltip("The maximum allowed speed that the ship is able to rotate on its axis.")] public float maximumAngularVelocity;
    [Tooltip("The maximum allowed amount of time that the ship can be out of bounds before it is destroyed.")] public float maximumBoundTime;
    [Tooltip("The value that the ship's thrust is multiplied by with the appropriate buff applied.")] public float thrustMultiplier;
    [Tooltip("The duration of invincibility granted to the player per each call.")] public float durationOfInvincibility;
    private float originalBoundTime;
    private float softBorderPosition;
    private float thrustInsert;
    private float originalInvincibilityDuration;

    [Header("Boolean Variables")]
    [Tooltip("Is the ship invincible?")] public bool isInvincible;
    [Tooltip("Is the ship currently in danger of being timed out?")] public bool isOutOfBounds;
    private bool shipIsActivelyMoving = false;
    private bool shipIsActivelyRotating = false;
    private bool hasAppliedInvincibility = false;

    void Awake()
    {

        // If the spaceship variable does not have a Rigidbody attached as a component, then this script will self-destruct
        if (cockpitObject == null) Destroy(this);

        // If the border object is left null, then this script will self-destruct
        if (borderMarker == null) Destroy(this);

    }

    void Start()
    {

        // If the ship's speed values are configured incorrectly, then this will fix them
        if (shipSpeed == 0f) shipSpeed = 1f;
        else if (shipSpeed < 0f) shipSpeed *= -1f;
        if (rotationSpeed == 0f) rotationSpeed = 2f;
        else if (rotationSpeed < 0f) rotationSpeed *= -1f;
        if (straightLineDamping == 0f) straightLineDamping = 1f;
        else if (straightLineDamping < 0f) straightLineDamping *= -1f;
        if (rotationalDamping == 0f) rotationalDamping = 1f;
        else if (rotationalDamping < 0f) rotationalDamping *= -1f;
        if (maximumLinearVelocity == 0f) maximumLinearVelocity = 10f;
        else if (maximumAngularVelocity < 0f) maximumAngularVelocity *= -1f;
        if (maximumAngularVelocity == 0f) maximumAngularVelocity = 0.5f;
        else if (maximumAngularVelocity < 0f) maximumAngularVelocity *= -1f;
        if (maximumBoundTime == 0f) maximumBoundTime = 5f;
        else if (maximumBoundTime < 0f) maximumBoundTime *= -1f;
        if (thrustMultiplier == 0f) thrustMultiplier = 2f;
        else if (thrustMultiplier < 0f) thrustMultiplier *= -1f;
        if (durationOfInvincibility == 0f) durationOfInvincibility = 1f;
        else if (durationOfInvincibility < 0f) durationOfInvincibility *= -1f;

        // Sets the original bound time to the value entered by the user
        originalBoundTime = maximumBoundTime;

        // Records the entered duration of invincibility
        originalInvincibilityDuration = durationOfInvincibility;

        // Records the X-position of the marker object, using that as the general bound for the square arena
        softBorderPosition = borderMarker.transform.position.x;

        // If the out-of-bounds Boolean is set to true, then this will correct that
        if (isOutOfBounds) isOutOfBounds = false;

        // If the player is not checked to have invincibility frames upon startup, then this will correct that
        if (!isInvincible) isInvincible = true;

        // Enables the input actions for inputs by default
        maneuverUp.Enable();
        maneuverDown.Enable();
        maneuverLeft.Enable();
        maneuverRight.Enable();
        rotateClockwise.Enable();
        rotateCounterclockwise.Enable();

    }

    void FixedUpdate()
    {

        // If the ship has linear or angular controls actively applied, then the bool denoting so will return true; otherwise, it will return false
        if (maneuverUp.IsPressed() || maneuverDown.IsPressed() || maneuverLeft.IsPressed() || maneuverRight.IsPressed() ||
            OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) ||
            OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) shipIsActivelyMoving = true;
        else shipIsActivelyMoving = false;
        if (rotateClockwise.IsPressed() || rotateCounterclockwise.IsPressed() ||
            OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)) shipIsActivelyRotating = true;
        else shipIsActivelyRotating = false;

        // If the game is using virtual reality controls
        if (gcReference.useXRControls)
        {

            // Activates the next method; the argument is false, indicating that VR inputs are used
            BindingControl(false);

        }
        // Otherwise, if the game is using conventional controls
        else
        {

            // Activates the next method; the argument is true, indicating that keyboard controls are used for input
            BindingControl(true);

        }

        // If the player has certain buffs applied, then float values will change as necessary
        if (pubReference.ptCheck) thrustInsert = thrustMultiplier;
        else thrustInsert = 1.0f;

        // If the player is out of bounds
        if (isOutOfBounds)
        {

            // If the timer has time left on it
            if (maximumBoundTime > 0f)
            {

                // The timer begins counting down
                maximumBoundTime -= Time.fixedDeltaTime;

                // Causes the vignette to slowly fade in via a mathematical calculation (this is inversely proportional to the time remaining)
                if (warningVignette.color.a < 1f) warningVignette.color = new(warningVignette.color.r, warningVignette.color.g,
                    warningVignette.color.b, (originalBoundTime - maximumBoundTime) * (1f / originalBoundTime));

            }
            // If the timer reaches zero
            else
            {

                // The vignette disappears completely
                warningVignette.color = new(warningVignette.color.r, warningVignette.color.g, warningVignette.color.b, 0f);

                // The player loses a life
                DestroyPlayer();

                // Sets the danger Boolean to false
                isOutOfBounds = false;

            }

            // Updates the timer in the TMP script and converts the current float value to an integer
            tmpReference.countdownText.text = Mathf.FloorToInt(maximumBoundTime).ToString();

        }
        // If the player is in bounds
        else
        {
            // If the vignette is still visible, then it will gradually vanish
            if (warningVignette.color.a > 0f) warningVignette.color = new(warningVignette.color.r, warningVignette.color.g,
                warningVignette.color.b, warningVignette.color.a - Time.fixedDeltaTime);

        }

        // If the ship is moving too quickly, then the methods to slow it down will activate
        if (cockpitObject.linearVelocity.x > maximumLinearVelocity * thrustInsert || cockpitObject.linearVelocity.x < -maximumLinearVelocity * thrustInsert) OverspeedControlX();
        if (cockpitObject.linearVelocity.z > maximumLinearVelocity * thrustInsert|| cockpitObject.linearVelocity.z < -maximumLinearVelocity * thrustInsert) OverspeedControlZ();
        if (cockpitObject.angularVelocity.y > maximumAngularVelocity * thrustInsert || cockpitObject.angularVelocity.y < -maximumAngularVelocity * thrustInsert) TorqueControl();

        // If the ship has the appropriate buff applied and it is in motion with no button input, then it will begin to slow to a stop
        if (pubReference.csCheck)
        {

            if (!shipIsActivelyMoving) cockpitObject.linearDamping = straightLineDamping;
            else cockpitObject.linearDamping = 0f;
            if (!shipIsActivelyRotating) cockpitObject.angularDamping = rotationalDamping;
            else cockpitObject.angularDamping = 0f;

        }
        // Otherwise, it will obey space physics as normal
        else
        {

            cockpitObject.linearDamping = 0f;
            cockpitObject.angularDamping = 0f;

        }

        // Manages the player's invincibility frames
        InvincibilityControl();

    }

    private void BindingControl(bool conventional)
    {

        if (conventional)
        {

            // If a given binding is pressed, it will activate its associated method below
            if (maneuverUp.IsPressed()) ManeuverShipForward();
            if (maneuverDown.IsPressed()) ManeuverShipBackward();
            if (maneuverLeft.IsPressed()) ManeuverShipLeft();
            if (maneuverRight.IsPressed()) ManeuverShipRight();
            if (rotateClockwise.IsPressed()) RotateShipClockwise();
            if (rotateCounterclockwise.IsPressed()) RotateShipCounterclockwise();

        }
        else
        {

            // If a VR binding is pressed, it will activate its associated method below
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)) ManeuverShipForward();
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown)) ManeuverShipBackward();
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)) ManeuverShipLeft();
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) ManeuverShipRight();
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft)) RotateShipClockwise();
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)) RotateShipCounterclockwise();

        }

    }

    private void ManeuverShipForward()
    {

        // Adds a force relative to the ship's local rotation in the proper direction, causing it to thrust smoothly
        cockpitObject.AddRelativeForce(shipSpeed * thrustInsert * Time.fixedDeltaTime * Vector3.forward);

    }

    private void ManeuverShipBackward()
    {

        // Adds a force relative to the ship's local rotation in the proper direction, causing it to thrust smoothly
        cockpitObject.AddRelativeForce(shipSpeed * thrustInsert * Time.fixedDeltaTime * Vector3.back);

    }

    private void ManeuverShipLeft()
    {

        // Adds a force relative to the ship's local rotation in the proper direction, causing it to thrust smoothly
        cockpitObject.AddRelativeForce(shipSpeed * thrustInsert * Time.fixedDeltaTime * Vector3.left);

    }

    private void ManeuverShipRight()
    {

        // Adds a force relative to the ship's local rotation in the proper direction, causing it to thrust smoothly
        cockpitObject.AddRelativeForce(shipSpeed * thrustInsert * Time.fixedDeltaTime * Vector3.right);

    }

    private void RotateShipClockwise()
    {

        // Applies torque (rotational force) in the proper direction, causing the ship to turn
        cockpitObject.AddRelativeTorque(rotationSpeed * thrustInsert * Time.fixedDeltaTime * Vector3.down);

    }

    private void RotateShipCounterclockwise()
    {

        // Applies torque (rotational force) in the proper direction, causing the ship to turn
        cockpitObject.AddRelativeTorque(rotationSpeed * thrustInsert * Time.fixedDeltaTime * Vector3.up);

    }

    private void OverspeedControlX()
    {

        // Limits the ship's linear velocity on the X-axis to the defined maximum (this works for both positive and negative thresholds)
        if (cockpitObject.linearVelocity.x > 0f) cockpitObject.linearVelocity = new Vector3((maximumLinearVelocity * thrustInsert) - 0.1f, 0f, cockpitObject.linearVelocity.z);
        else cockpitObject.linearVelocity = new Vector3((-maximumLinearVelocity * thrustInsert) + 0.1f, 0f, cockpitObject.linearVelocity.z);

    }

    private void OverspeedControlZ()
    {

        // Limits the ship's linear velocity on the Z-axis to the defined maximum (this works for both positive and negative thresholds)
        if (cockpitObject.linearVelocity.z > 0f) cockpitObject.linearVelocity = new Vector3(cockpitObject.linearVelocity.x, 0f, (maximumLinearVelocity * thrustInsert) - 0.1f);
        else cockpitObject.linearVelocity = new Vector3(cockpitObject.linearVelocity.x, 0f, (-maximumLinearVelocity * thrustInsert) + 0.1f);

    }

    private void TorqueControl()
    {

        // Limits the ship's angular velocity to the defined maximum (this works for both positive and negative thresholds)
        if (cockpitObject.angularVelocity.y > 0f) cockpitObject.angularVelocity = new Vector3(0f, (maximumAngularVelocity * thrustInsert) - 0.1f, 0f);
        else cockpitObject.angularVelocity = new Vector3(0f, (-maximumAngularVelocity * thrustInsert) + 0.1f, 0f);

    }

    private void InvincibilityControl()
    {

        /* If the player is currently invincible, then the power will proceed as normal;
        otherwise, the check (kill Boolean) will reset once the player regains vulnerability */
        if (isInvincible)
        {

            // If the invincibility timer has not been refreshed yet
            if (!hasAppliedInvincibility)
            {

                // Loads the prescripted amount of time into the timer
                durationOfInvincibility = originalInvincibilityDuration;

                // Switches the kill Boolean to true
                hasAppliedInvincibility = true;

            }

            /* If the invincibility timer still has a greater-than-zero value, then it will subtract using unscaled time;
            otherwise, the Boolean will become false */
            if (durationOfInvincibility > 0f) durationOfInvincibility -= Time.fixedDeltaTime;
            else isInvincible = false;

        }
        else hasAppliedInvincibility = false;

    }

    void OnTriggerEnter(Collider softBorder)
    {
        
        // If the collider has the correct tag
        if (softBorder.CompareTag("Soft Border"))
        {

            // For-loop that repeats for each element in the warning TMP text array
            for (int i = 0; i < tmpReference.warningTexts.Length; i++)
            {

                // Causes each line to type into view
                tmpReference.RemoteStart(tmpReference.warningTexts[i], true);

            }
            tmpReference.RemoteStart(tmpReference.countdownText, true);

        }

    }

    void OnTriggerStay(Collider softBorder)
    {
        
        // If the collider has the correct tag
        if (softBorder.CompareTag("Soft Border"))
        {

            // Changes the Boolean to true
            if (!isOutOfBounds) isOutOfBounds = true;

        }

    }

    void OnTriggerExit(Collider softBorder)
    {

        // If the collider has the correct tag
        if (softBorder.CompareTag("Soft Border"))
        {

            // Changes the Boolean to false
            if (isOutOfBounds) isOutOfBounds = false;

            // If the player is close enough to the arena center
            if (cockpitObject.transform.position.x < softBorderPosition && cockpitObject.transform.position.z < softBorderPosition &&
                cockpitObject.transform.position.x > -softBorderPosition && cockpitObject.transform.position.x > -softBorderPosition)
            {

                // Sets the danger timer back to its original value
                maximumBoundTime = originalBoundTime;

                // For-loop that repeats for each element in the warning TMP text array
                for (int j = 0; j < tmpReference.warningTexts.Length; j++)
                {

                    // Causes each line to disappear from view
                    tmpReference.RemoteStart(tmpReference.warningTexts[j], false);

                }
                tmpReference.RemoteStart(tmpReference.countdownText, false);

            }
                

        }

    }

    void OnCollisionEnter(Collision obstacle)
    {

        // If the ship collides with an asteroid (as marked by the proper tag)
        if (obstacle.gameObject.CompareTag("Asteroid"))
        {

            // Causes the asteroid to fragment if it is larger than the smallest size
            if (obstacle.gameObject.GetComponent<AsteroidBehavior>().asteroidSize > 0) obstacle.gameObject.GetComponent<AsteroidMitosis>().EjectFragments();

            /* If the player is not invincible, then the player will be destroyed;
            otherwise, points for destroying the asteroid will be granted to the player */
            if (!isInvincible) DestroyPlayer();
            else lcReference.playerScore += obstacle.gameObject.GetComponent<AsteroidBehavior>().pointsToAdd;

            // Destroys the asteroid as a missile or bullet impact would
            Destroy(obstacle.gameObject);

        }
        // If the ship collides with the death border (as marked by the proper tag), then player control is taken away
        else if (obstacle.gameObject.CompareTag("Hard Border")) DestroyPlayer();

    }

    public void DestroyPlayer()
    {

        // Removes all buffs currently applied to the player through the referenced script
        pubReference.RemoveAllBuffs();

        // If the player was previously out of bounds, then this will be nullified
        if (maximumBoundTime < originalBoundTime)
        {

            // If the vignette is visible, then it will vanish immediately
            if (warningVignette.color.a > 0f) warningVignette.color = new(warningVignette.color.r, warningVignette.color.g, warningVignette.color.b, 0f);

            // For-loop that repeats for each element in the warning TMP text array
            for (int j = 0; j < tmpReference.warningTexts.Length; j++)
            {

                // Causes each line to disappear from view
                tmpReference.RemoteStart(tmpReference.warningTexts[j], false);

            }
            tmpReference.RemoteStart(tmpReference.countdownText, false);

        }

        // If the player was out of bounds at the time of death, then the timer and Boolean will be reverted
        if (isOutOfBounds) isOutOfBounds = false;
        if (maximumBoundTime < originalBoundTime) maximumBoundTime = originalBoundTime;

        // The player will lose a life
        lcReference.playerLives--;

        // Plays the death sequence, passing the player's location as the location of death beforehand
        dsReference.deathLocation = cockpitObject.position;
        dsReference.PlayDeathSequence();

        // The lost life will be appended to the counter
        lcReference.livesLostDuringRound++;

        // Deactivates the spaceship (this object)
        gameObject.SetActive(false);

    }

}
