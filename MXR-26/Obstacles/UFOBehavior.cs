/* Charlie Dye - ECT 4440 - 2026.02.12

This is the script for active UFO movement */

using System.Collections;
using UnityEngine;

public class UFOBehavior : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The enum that stores the values for the UFO's moving module behavior.")] public UFOEnumMoving uemReference;

    [Header("Objects")]
    [Tooltip("The physical body of the UFO.")] public GameObject shipBody;
    [Tooltip("The explosion that occurs when a UFO is destroyed.")] public GameObject hostileShipExplosion;
    private GameObject playerController;

    [Header("Numerical Variables")]
    [Tooltip("The base speed of the UFO.")] public float shipSpeedBase;
    [Tooltip("The amount to multiply the base speed by if the UFO is large.")] public float shipSpeedMultiplier;
    [Tooltip("The rate at which the UFO's body rotates.")] public float bodyRotationRate;
    [Tooltip("The maximum distance from the X- and Z- axes that a UFO can choose to move towards if moving randomly.")] public float maximumRandomDistance;
    [Tooltip("The refresh rate of the location-seeking coroutine.")] public float locateRefreshRate;
    [Tooltip("The refresh rate of the movement coroutine.")] public float movementRefreshRate;
    [Tooltip("The amount of times the UFO's location coroutine can be called before the UFO must despawn.")] public int maximumCallTimes;
    [Tooltip("The amount of points to be awarded to the player if this UFO is destroyed.")] public int pointsWorth;
    private int timesCalled = 0;
    private Vector3 initialSpawnLocation;
    private Vector3 locationToMoveTo;

    [Header("Audio")]
    [Tooltip("The \"blip\" that plays on the player's radar when it detects this object.")] public AudioSource radarBlip;

    [Header("Boolean Variable")]
    [Tooltip("Is the UFO large?")] public bool isLarge;

    void Awake()
    {

        // If the UFO does not contain a Rigidbody or collider, then this script will self-destruct
        if (!gameObject.GetComponent<Rigidbody>() || !gameObject.GetComponent<Collider>()) Destroy(this);

    }

    void OnEnable()
    {

        // If the numerical variables have improper values, then this will correct them
        if (shipSpeedBase == 0f) shipSpeedBase = 1f;
        else if (shipSpeedBase < 0f) shipSpeedBase *= -1f;
        if (shipSpeedMultiplier == 0f) shipSpeedMultiplier = 1.5f;
        else if (shipSpeedMultiplier < 0f) shipSpeedMultiplier *= -1f;
        if (maximumRandomDistance == 0f) maximumRandomDistance = 500f;
        else if (maximumRandomDistance < 0f) maximumRandomDistance *= -1f;
        if (locateRefreshRate == 0f) locateRefreshRate = 10f;
        else if (locateRefreshRate < 0f) locateRefreshRate *= -1f;
        if (movementRefreshRate == 0f) movementRefreshRate = 0.03f;
        else if (movementRefreshRate < 0f) movementRefreshRate *= -1f;
        if (maximumCallTimes == 0) maximumCallTimes = 10;
        else if (maximumCallTimes < 0) maximumCallTimes *= -1;
        if (pointsWorth < 0) pointsWorth = 100;

        // Attempts to fetch the private object by indexing the scene; if that fails, then this script will self-destruct
        playerController = FindFirstObjectByType<SpaceshipBehavior>().gameObject;

        // If the UFO is large and not small, then its speed will be multiplied; otherwise, it will be set to a default value of 1
        if (isLarge) shipSpeedMultiplier = 2f;
        else shipSpeedMultiplier = 1f;

        // Records the spawn location as the proper vector variable
        initialSpawnLocation = gameObject.transform.position;

        // Starts the coroutines
        StartCoroutine(SeekLocationTarget());
        StartCoroutine(MoveTowardsTarget());

    }

    void FixedUpdate()
    {
        
        // Creates rotation, causing the UFO to spin
        shipBody.transform.Rotate(bodyRotationRate * Time.fixedDeltaTime * Vector3.up);

    }

    public void CreateRadarBlip()
    {

        // If the ship's radar system hits this object (i.e., detects it with a trigger collider), then this object will play a spatialized blip
        radarBlip.Play();

    }

    void OnCollisionEnter(Collision impactingObject)
    {

        // If the UFO rams into the player (as properly tagged)
        if (impactingObject.gameObject.CompareTag("Player"))
        {

            // Creates the explosion
            Instantiate(hostileShipExplosion, shipBody.transform.position, Quaternion.identity);

            /* If the player is not invincible, then this destroys the player by accessing the script that is always attached to the player;
            otherwise, the player remains unharmed, and this UFO rewards points before being destroyed */
            if (!impactingObject.gameObject.GetComponent<SpaceshipBehavior>().isInvincible) impactingObject.gameObject.GetComponent<SpaceshipBehavior>().DestroyPlayer();
            else FindFirstObjectByType<LevelCounter>().playerScore += pointsWorth;

            // Destroys this UFO (object)
            Destroy(gameObject);

        }
        // If the UFO hits an asteroid (as properly tagged)
        else if (impactingObject.gameObject.CompareTag("Asteroid"))
        {

            // Creates the explosion
            Instantiate(hostileShipExplosion, shipBody.transform.position, Quaternion.identity);

            // Causes the asteroid to fragment if it is larger than the smallest size
            if (impactingObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize > 0) impactingObject.gameObject.GetComponent<AsteroidMitosis>().EjectFragments();

            // Destroys the asteroid as a missile or bullet impact would
            Destroy(impactingObject.gameObject);

            // Destroys this UFO (object)
            Destroy(gameObject);

        }

    }

    private IEnumerator SeekLocationTarget()
    {

        // While-loop that returns true as long as the UFO is not destroyed
        while (gameObject != null)
        {

            // If the UFO can select a new spot to move towards freely
            if (timesCalled < maximumCallTimes)
            {

                // If the UFO is targeting the player's position
                if (uemReference == UFOEnumMoving.Homing)
                {

                    // Records the player's position in the arena at the time of the call
                    locationToMoveTo = playerController.transform.position;

                    // Increments the private integer by one
                    timesCalled++;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(locateRefreshRate);

                }
                // If the UFO moves randomly
                else if (uemReference == UFOEnumMoving.Roaming)
                {

                    // Constructs a new vector using the given randomization parameters
                    locationToMoveTo = new Vector3(Random.Range(-maximumRandomDistance, maximumRandomDistance), 0f, Random.Range(-maximumRandomDistance, maximumRandomDistance));

                    // Increments the private integer by one
                    timesCalled++;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(locateRefreshRate);

                }

            }
            // If the UFO has reached its call limit and must despawn
            else
            {

                // Moves toward the initial spawn location
                Vector3.MoveTowards(gameObject.transform.position, initialSpawnLocation, shipSpeedBase * shipSpeedMultiplier);

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(movementRefreshRate);

            }

        }

    }

    private IEnumerator MoveTowardsTarget()
    {

        // While-loop that returns true as long as the UFO is not destroyed
        while (gameObject != null)
        {

            // If the UFO is not at its target location, then it will move there
            if (gameObject.transform.position != locationToMoveTo) gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, locationToMoveTo, shipSpeedBase * shipSpeedMultiplier);

            // Refreshes the coroutine
            yield return new WaitForSecondsRealtime(movementRefreshRate);

        }

    }

}
