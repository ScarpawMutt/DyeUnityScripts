/* Charlie Dye - ECT 4440 - 2026.02.13

This is the script for active UFO weapon systems */


using System.Collections;
using UnityEngine;

public class UFOWeapons : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script that references UFO movement. This script should always have a UFO movement script attached.")] public UFOBehavior ufobReference;
    [Tooltip("The enum that stores the values for the UFO's firing module behavior.")] public UFOEnumFiring uefReference;

    [Header("Weapon Objects")]
    [Tooltip("The cannon that the UFO shoots bullets from.")] public GameObject shipCannon;
    [Tooltip("The object that the UFO is aiming its cannon towards.")] public GameObject cannonTarget;
    [Tooltip("The randomized \"null\" target to be shot at if the UFO is a gunship.")] public GameObject nullObject;
    [Tooltip("The bullet prefab fired by the UFO.")] public GameObject shipBullet;
    [Tooltip("The location the bullet spawns at.")] public Transform bulletSpawn;

    [Header("Audio")]
    [Tooltip("The sound of the laser/railgun firing.")] public AudioSource railgunFire;

    [Header("Numerical Variables")]
    [Tooltip("The maximum distance relative to each axis that a randomized gunship target can be selected in.")] public float maximumTargetDeviation;
    [Tooltip("The maximum speed at which the UFO's cannon can pivot.")] public float cannonRotationRate;
    [Tooltip("The refresh rate of the cannon-rotating coroutine.")] public float rotateRefreshRate;
    [Tooltip("The refresh rate of the cannon-firing coroutine.")] public float fireRefreshRate;

    void Awake()
    {

        // If the UFO does not contain a movement script, then this script will self-destruct
        if (ufobReference == null) Destroy(this);

    }

    void OnEnable()
    {

        // If the numerical variables have improper values, then this will correct them
        if (maximumTargetDeviation < 0f) maximumTargetDeviation *= -1f;
        if (cannonRotationRate == 0f) cannonRotationRate = 1f;
        else if (cannonRotationRate < 0f) cannonRotationRate *= -1f;
        if (rotateRefreshRate == 0f) rotateRefreshRate = 0.03f;
        else if (rotateRefreshRate < 0f) rotateRefreshRate *= -1f;
        if (fireRefreshRate == 0f) fireRefreshRate = 1f;
        else if (fireRefreshRate < 0f) fireRefreshRate *= -1f;

        // Starts the coroutines
        StartCoroutine(RotateCannon());
        StartCoroutine(FireCannon());

    }

    private IEnumerator RotateCannon()
    {

        // While-loop that returns true as long as the ship's cannon is not null (this should ALWAYS return true)
        while (shipCannon != null)
        {

            // If the UFO is tracking a target
            if (cannonTarget != null)
            {

                // Introduces a local vector that measures the difference between the cannon and target transforms
                Vector3 targetDirection = cannonTarget.transform.position - shipCannon.transform.position;

                // Quaternion that forces the cannon to "look" in the direction of the target difference
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                // Creates a spherically-interpolated rotation, causing the cannon to follow the target
                shipCannon.transform.rotation = Quaternion.Slerp(shipCannon.transform.rotation, targetRotation, cannonRotationRate * Time.fixedDeltaTime);

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(rotateRefreshRate);

            }
            // If the UFO is not tracking a target, then the coroutine will simply refresh
            else yield return new WaitForSecondsRealtime(rotateRefreshRate);

        }

    }

    private IEnumerator FireCannon()
    {

        // If the UFO has inherited fighter behavior, then the player will be its target
        if (uefReference == UFOEnumFiring.Fighter) cannonTarget = GameObject.FindGameObjectWithTag("Player");
        // If the ship has inherited gunship behavior, then it will set up its proper behavior in the associated method
        else if (uefReference == UFOEnumFiring.Gunship) RandomReposition();
        // If the ship has inherited minesweeper behavior, then it will set up its proper behavior in the associated method
        else if (uefReference == UFOEnumFiring.Minesweeper) RefreshTargets();

        // While-loop that returns true as long as the ship's cannon is not null (this should ALWAYS return true)
        while (shipCannon != null)
        {

            // If the UFO is tracking an activated target (this is to prevent it from spawning extraneous bullets if the player is deactivated and the UFO's target)
            if (cannonTarget != null && cannonTarget.activeInHierarchy)
            {

                // Plays audio
                railgunFire.Play();

                // Instantiates the bullet prefab in the direction the cannon is facing
                Instantiate(shipBullet, bulletSpawn.position, shipCannon.transform.rotation);

                // If the UFO is a gunship, then it will reset its random shooting target
                if (uefReference == UFOEnumFiring.Gunship) RandomReposition();

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(fireRefreshRate);

            }
            // If the UFO is not tracking a target
            else
            {

                // If the UFO is a minesweeper, then it will refresh its array of possible targets
                if (uefReference == UFOEnumFiring.Minesweeper) RefreshTargets();

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(fireRefreshRate);

            }

        }

    }

    private void RandomReposition()
    {

        // Sets up the "null" object and cannon target if necessary; then, it assigns the variables properly
        if (nullObject == null && GameObject.FindGameObjectWithTag("Random Target")) nullObject = GameObject.FindGameObjectWithTag("Random Target");
        if (cannonTarget == null) cannonTarget = nullObject;

        // Moves the random target to a randomized coordinate as defined by the value entered in
        nullObject.transform.position = new Vector3(Random.Range(-maximumTargetDeviation, maximumTargetDeviation), 0f, Random.Range(-maximumTargetDeviation, maximumTargetDeviation));

    }

    private void RefreshTargets()
    {

        // If at least one asteroid is in the arena
        if (GameObject.FindGameObjectWithTag("Asteroid"))
        {

            // Creates an array of all asteroids currently in the scene
            GameObject[] activeAsteroids = GameObject.FindGameObjectsWithTag("Asteroid");

            // Targets the first element retrieved
            cannonTarget = activeAsteroids[0];

        }

    }

}
