/* Charlie Dye, PACE Team - 2024.09.26

This is the script for the Minuteman Missile */

using System.Collections;
using TestMQTT;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the blinding effect on the player's HUD.")] public LossTransition ltReference;
    [Tooltip("The script attached to the lugnut puzzle.")] public NutBehavior nbReference;
    [Tooltip("The script attached to the new rendition of the silo.")] public NewSiloBehavior nsbReference;
    [Tooltip("The script attached to the player's timer.")] public TimerBehavior tbReference;
    public ButtKickerManager ButtKickerManager;

    [Header("Rigidbody Variable")]
    [Tooltip("The rigidbody of the ladder leaning against the missile.")] public Rigidbody ladderRigidbody;

    [Header("Audio Parameters")]
    [Tooltip("The audio source to play when the missile rumbles.")] public AudioSource missileRumbling;
    [Tooltip("The audio source to play as the missile lifts off.")] public AudioSource missileLaunching;

    [Header("Particle System Arrays")]
    [Tooltip("The array of particle systems containing the missile's exhaust steam.")] public ParticleSystem[] missileExhausts;
    [Tooltip("The array of particle systems containing the missile's thruster exhaust.")] public ParticleSystem[] missilePropulsions;
    [Tooltip("The array of particle systems containing the exhausted flames from the background smokestacks.")] public ParticleSystem[] exhaustFlames;

    [Header("Transform Variable")]
    public Transform missileTerminus;
    private Vector3 originalMissileLocation;

    [Header("Float Variables")]
    [Tooltip("How fast the missile shakes.")] public float shakingIntensity;
    [Tooltip("How fast the missile resets to its default position once it stops shaking.")] public float resetSpeed;
    [Tooltip("The initial speed of the missile once it lifts off.")] public float launchSpeed;
    [Tooltip("How fast the missile accelerates as it launches.")] public float launchAcceleration;
    [Tooltip("The duration of the missile shaking just before it launches.")] public float timeUntilLaunch;
    [Tooltip("The duration to wait until the silo closes its lid once the missile is airborne.")] public float launchDuration;
    [Tooltip("The minimum height the missile must be at before its audio begins to diminish.")] public float altitudeOfAttenuation;
    [Tooltip("How fast the audio diminishes once the missile is far enough away.")] public float diminishRate;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;
    private float randomPositionX;
    private float randomPositionZ;

    [Header("Boolean Variables")]
    [Tooltip("Is the missile animated?")] public bool awakenMissile = false;
    [Tooltip("Should the missile emit steam?")] public bool emitSteam = false;
    private bool missileIsHighEnough = false;
    private bool missileHasAwakened;
    private bool missileHasCalmed;
    private bool steamHasStarted;
    private bool steamHasStopped;

    void Start()
    {

        // Records the missile's location as it is loaded in
        originalMissileLocation = gameObject.transform.position;

        // If the ladder's rigidbody is not set properly, then it will correct itself
        if (!ladderRigidbody.isKinematic) ladderRigidbody.isKinematic = true;
        if (ladderRigidbody.interpolation != RigidbodyInterpolation.Interpolate)
            ladderRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        if (ladderRigidbody.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
            ladderRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

    }

    void FixedUpdate()
    {

        // If the missile is exhausting steam, then it will shake
        if (awakenMissile) ShakeMissile();

        /* If the missile has alreadly launched and high enough in elevation, its powerplant sound will begin to diminish;
        this will continue as long as the missile has not been removed or its volume is greater than zero, whichever comes first */
        if (gameObject.transform.position.y >= altitudeOfAttenuation && !missileIsHighEnough)
        {

            StartCoroutine(AttenuateExhaust());
            missileIsHighEnough = true;

        }

        // Constantly checks the activation status of the missile
        StatusCheck();

    }

    private void StatusCheck()
    {

        // If the missile is activated
        if (awakenMissile)
        {

            // If the awaken kill Boolean has not yet switched
            if (!missileHasAwakened)
            {

                // Plays the rumbling audio
                missileRumbling.Play();
                

                // Inverts the values of the kill Booleans
                missileHasAwakened = true;
                missileHasCalmed = false;
                ButtKickerManager.PlayButtKicker("Missile", 1, 2, true);
                Debug.Log("Missile Rumble On");

            }

        }
        // If the missile is not active
        else
        {

            // If the dormant kill Boolean has not yet activated
            if (!missileHasCalmed)
            {

                // Repositions the missile
                RepositionMissile();

                // Stops the rumbling audio
                missileRumbling.Stop();

                // Inverts the values of the kill Boolean
                missileHasAwakened = false;
                missileHasCalmed = true;

            }

        }

        // If steam is being emitted
        if (emitSteam)
        {

            // If the start kill Boolean has not yet activated
            if (!steamHasStarted)
            {

                // For-loop that runs through each element in the exhaust array, starting the particle system in each
                for (int i = 0; i < missileExhausts.Length; i++)
                {

                    missileExhausts[i].Play();

                }

                // Inverts the values of the kill Booleans
                steamHasStarted = true;
                steamHasStopped = false;

            }

        }
        // If steam is not being emitted
        else
        {

            // If the stop kill Boolean has not yet activated
            if (!steamHasStopped)
            {

                // For-loop that runs through each element in the exhaust array, stopping the particle system in each 
                for (int j = 0; j < missileExhausts.Length; j++)
                {

                    missileExhausts[j].Stop();

                }

                // Inverts the values of the kill Booleans
                steamHasStarted = false;
                steamHasStopped = true;

            }

        }

    }

    private void ShakeMissile()
    {

        // Creates randomized position and rotation float values
        randomPositionX = originalMissileLocation.x + Random.Range(-shakingIntensity, shakingIntensity);
        randomPositionZ = originalMissileLocation.z + Random.Range(-shakingIntensity, shakingIntensity);

        // Creates a new Vector3 variable based on earlier values
        gameObject.transform.position = new(randomPositionX, gameObject.transform.position.y, randomPositionZ);

    }

    private void RepositionMissile()
    {

        // If the missile's coordinates are not equivalent to the original Vector-3 recorded, then it will be returned to that position
        if (gameObject.transform.position != originalMissileLocation) gameObject.transform.position = originalMissileLocation;

    }

    public void InitiateLaunchSequence()
    {

        // Makes the missile rumble
        awakenMissile = true;

        // Ignites the missile's engines
        for (int k = 0; k < missilePropulsions.Length; k++)
        {

            missilePropulsions[k].Play();

        }

        // Transitions to the coroutine after a set amount of time
        Invoke(nameof(TransitionToLiftoff), timeUntilLaunch);

    }

    private void TransitionToLiftoff()
    {

        // Stops the missile from rumbling
        awakenMissile = false;

        // Controls the flames from the exhaust duct
        for (int l = 0; l < exhaustFlames.Length; l++)
        {

            exhaustFlames[l].Play();

        }

        // Begins the coroutine
        StartCoroutine(MissileLiftoff());

        // Sets the timer to finish the silo's liftoff sequence
        Invoke(nameof(ConcludeLiftoff), launchDuration);

    }

    private void ConcludeLiftoff()
    {

        // Ends the flame particle system from the exhaust
        for (int m = 0; m < exhaustFlames.Length; m++)
        {

            exhaustFlames[m].Stop();

        }

        // Makes the silo close its lid
        if (!nsbReference.siloIsOpen) nsbReference.siloIsOpen = true;
        nsbReference.executeCoverAnimation = true;

    }

    private IEnumerator MissileLiftoff()
    {

        // Alters noise
        missileRumbling.Stop();
        missileLaunching.Play();

        // If the game has not been won and softlocked, then the blinding effect will begin to take over after a delay
        if (!nbReference.winConditionHasExecuted) Invoke(nameof(LossLaunch), timeUntilLaunch);

        // Removes the ladder's kinematic property, causing it to fall
        ladderRigidbody.isKinematic = false;

        while (gameObject.transform.position != missileTerminus.transform.position)
        {

            // Accelerates the missile to the end position by adding onto the acceleration float
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, missileTerminus.transform.position, (launchSpeed += launchAcceleration) * Time.deltaTime);

            yield return new WaitForSecondsRealtime(0.01f);

        }

        yield return null;

        // Deactivates the gameObject
        gameObject.SetActive(false);

    }

    private void LossLaunch()
    {

        // References the blinding script
        ltReference.playerHasLost = true;

    }

    private IEnumerator AttenuateExhaust()
    {

        while (gameObject != null)
        {

            if (missileLaunching.volume > 0f)
            {

                // Lowers the missile's volume for a smooth sonic transition
                missileLaunching.volume -= diminishRate;

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else yield break;

        }

    }

}
