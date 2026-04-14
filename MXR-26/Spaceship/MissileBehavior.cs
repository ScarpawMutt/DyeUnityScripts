/* Charlie Dye - ECT 4440 - 2026.01.25

This is the script for controlling the spawned missile projectiles */

using UnityEngine;

public class MissileBehavior : MonoBehaviour
{

    [Header("Special Effects")]
    [Tooltip("The explosion that occurs when the missile hits an asteroid, created by the object itself.")] public GameObject asteroidExplosion;
    [Tooltip("The explosion that occurs when the missile hits a UFO, created by the object itself.")] public GameObject hostileShipExplosion;
    [Tooltip("The explosion that occurs when the missile hits any object, created by this projectile.")] public GameObject projectileExplosion;
    [Tooltip("The prefab that creates the explosion audio.")] public GameObject explosionModule;

    [Header("Float Variable")]
    [Tooltip("The linear speed of the missile once fired.")] public float missileSpeed;
    [Tooltip("The turning speed of the missile once fired.")] public float turningSpeed;
    [Tooltip("The value the speed of the bullet is multiplied by if the appropriate buff is applied.")] public float speedMultiplier;
    [Tooltip("The time before audio and special effects begin playing.")] public float timeToBeginEffects;
    [Tooltip("The missile's minimum lifespan before its accuracy begins to increase.")] public float timeToIncreaseAccuracy;
    [Tooltip("The gradual increase in accuracy as the missile tracks.")] public float accuracyIncrement;
    [Tooltip("The delay, once fired, before the missile gains the ability to home in on a target.")] public float lockDelay;
    [Tooltip("The maximum amount of time a missile can lock onto an object before it self-destructs.")] public float detonationTimer;
    [Tooltip("The amount of time the object exists in Unity before being destroyed. This is so that the attached particle system functions correctly.")] public float postmortemTimer;
    private float speedInsert;

    // Script reference
    private LevelCounter lcReference;
    private PowerUpBank pubReference;
    private SpaceshipBehavior sbReference;

    // Object variable
    private GameObject missileTarget;

    // Rigidbody variable
    private Rigidbody missileRigidbody;

    // Audio source variable
    private AudioSource missileAudio;

    // Particle system variable
    private ParticleSystem missileExhaust;

    // Boolean variables
    [HideInInspector] public bool missileHasImpacted = false;
    private bool missileCanLockOn = false;
    [SerializeField] private bool effectsAreActive = false;

    void Awake()
    {

        /* Scans the scene for objects with the appropriate scripts (this should always return true);
        as long as it does, it will assign the private script references with what it finds */
        if (FindFirstObjectByType<SpaceshipBehavior>()) sbReference = FindFirstObjectByType<SpaceshipBehavior>();
        if (FindFirstObjectByType<PowerUpBank>()) pubReference = FindFirstObjectByType<PowerUpBank>();
        if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();

        // Sets the private variables as the missile's own
        missileRigidbody = gameObject.GetComponent<Rigidbody>();
        missileAudio = gameObject.GetComponentInChildren<AudioSource>();
        missileExhaust = gameObject.GetComponentInChildren<ParticleSystem>();

        // If the effect objects are active and there is a set timer, then they will be switched off for now
        if (missileAudio.gameObject.activeInHierarchy && timeToBeginEffects > 0f) missileAudio.gameObject.SetActive(false);
        if (missileExhaust.gameObject.activeInHierarchy && timeToBeginEffects > 0f) missileExhaust.gameObject.SetActive(false);

    }

    void OnEnable()
    {

        // Ignores collision with objects that belong in the layer for either the player (3) or other projectiles (6)
        Physics.IgnoreLayerCollision(3, 6);
        Physics.IgnoreLayerCollision(6, 6);

        // If the player is powered up appropriately, then the bullet's speed will be multiplied
        if (pubReference.fbCheck) speedInsert = speedMultiplier;
        else speedInsert = 1.0f;

        // Causes the bullet to move forward in a straight line relative to its local rotation at a constant rate; the velocity of the ship is preserved
        missileRigidbody.AddRelativeForce(missileSpeed * speedInsert * Time.fixedDeltaTime * Vector3.forward + sbReference.cockpitObject.linearVelocity);

        // If the float variables are configured incorrectly on the parent prefab, then this will fix them
        if (missileSpeed == 0f) missileSpeed = 20000f;
        else if (missileSpeed < 0f) missileSpeed *= -1f;
        if (turningSpeed == 0f) turningSpeed = 100f;
        else if (turningSpeed < 0f) turningSpeed *= -1f;
        if (detonationTimer == 0f) detonationTimer = 20f;
        else if (detonationTimer < 0f) detonationTimer *= -1f;
        if (postmortemTimer == 0f) postmortemTimer = 1f;
        else if (postmortemTimer < 0f) postmortemTimer *= -1f;

        // Immediately upon being launched, the missile determines the closest asteroid (i.e., the shortest straight-line distance)
        FindNearestObstacle();

    }

    private GameObject FindNearestObstacle()
    {

        // If a UFO is actively in the arena, then it automatically takes priority over any asteroids
        if (GameObject.FindGameObjectWithTag("Hostile Spacecraft")) missileTarget = GameObject.FindGameObjectWithTag("Hostile Spacecraft");
        // If no UFO is present
        else
        {

            // Local object array that indexes all properly tagged obstacles, converting the retrieved array into itself
            GameObject[] allPotentialTargets = GameObject.FindGameObjectsWithTag("Asteroid");

            // Local null object variable that stores the missile's ultimate target
            GameObject currentTarget = null;

            // Local float array that stores the distances of the potential targets
            float[] targetDistances = new float[allPotentialTargets.Length];

            for (int i = 0; i < allPotentialTargets.Length; i++)
            {

                // Local Vector-3 variable that subtracts the asteroid's position from the missile's position
                Vector3 vectoredDistance = allPotentialTargets[i].transform.position - gameObject.transform.position;

                // Takes the square magnitude of the result, storing each calculation in the corresponding float array
                targetDistances[i] = vectoredDistance.sqrMagnitude;

            }

            for (int j = 0; j < targetDistances.Length; j++)
            {

                /* If the decimal value is less than the one preceding it, then it will become the missile's target
                this may change as the array is fully indexed and even smaller values are located */
                if (j > 0)
                {

                    if (targetDistances[j] < targetDistances[j - 1]) currentTarget = allPotentialTargets[j];

                }
                else if (j == 0) currentTarget = allPotentialTargets[j];

            }

            // Finalizes the target value
            missileTarget = currentTarget;

        }

        // Returns the result
        return missileTarget;

    }

    void FixedUpdate()
    {

        // Immediately counts down from the given delay
        if (lockDelay > 0f && !missileCanLockOn)
        {

            lockDelay -= Time.fixedDeltaTime;

        }
        else missileCanLockOn = true;

        // Immediately counts down until the effects can begin
        if (timeToBeginEffects > 0f) timeToBeginEffects -= Time.fixedDeltaTime;
        else
        {

            // If the effects have not yet begun but must do so
            if (!effectsAreActive)
            {

                // Plays the audio and special effects
                missileAudio.gameObject.SetActive(true);
                missileExhaust.gameObject.SetActive(true);

                // Switches the kill Boolean on
                effectsAreActive = true;

            }

        }

        // If the missile has located a target
        if (missileCanLockOn)
        {

            // Steers the missile towards the target
            LockOntoTarget();

            // Begins the detonation timer and accuracy measures if the values are configured (this is an optional behavior)
            if (timeToIncreaseAccuracy > 0f && accuracyIncrement > 0f) timeToIncreaseAccuracy -= Time.fixedDeltaTime;
            detonationTimer -= Time.fixedDeltaTime;

        }

        // If the missile has impacted its target
        if (missileHasImpacted)
        {

            // If the timer still has time left
            if (postmortemTimer > 0f)
            {

                // Subtracts unscaled time from the timer
                postmortemTimer -= Time.fixedDeltaTime;

                // Stops the particle system in its child object from emitting particles
                gameObject.GetComponentInChildren<ParticleSystem>().Stop();

                // Turns the missile's body invisible by disabling its renderer
                gameObject.GetComponent<Renderer>().enabled = false;

                // Freezes the missile's position and rotation
                missileRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            }
            // Otherwise, the object will be destroyed outright
            else Destroy(gameObject);

        }

        // If the missile has tracked a target for a given long enough time, then it will begin to home in more severely
        if (timeToIncreaseAccuracy < 0f) BetterSteer();

        /* If the missile has tracked a target for too long without hitting anything, then it will detonate on its own;
        this is necessary because missiles can miss smaller targets, and fast-moving ones can outrun them */
        if (detonationTimer < 0f) missileHasImpacted = true;

    }

    private void LockOntoTarget()
    {

        // If the target is not null (i.e., the target asteroid has not been destroyed)
        if (missileTarget != null)
        {

            // Gets the difference between the two vectors
            Vector3 targetDirection = missileTarget.transform.position - gameObject.transform.position;

            // Normalizes the result
            targetDirection.Normalize();

            // Takes the perpendicular result of the two vectors, creating a smooth curve in the missile's trajectory
            Vector3 rotationAmount = Vector3.Cross(transform.forward, targetDirection);

            // Sets the missile's turning radius to be a function of the "Cross" vector
            missileRigidbody.angularVelocity = (rotationAmount * speedInsert) * (turningSpeed * speedInsert);

            // Moves the missile along at a constant speed (the division by two thousand is necessary for this effect)
            missileRigidbody.linearVelocity = transform.forward * (missileSpeed / 2000f);

        }

    }

    private void BetterSteer()
    {

        // Decreases the missile's turning radius by adding to it as a function of fixed framerate time
        turningSpeed += Time.fixedDeltaTime * accuracyIncrement;

    }

    void OnCollisionEnter(Collision impactingObject)
    {

        // If the colliding object is an asteroid (as appropriately tagged)
        if (impactingObject.gameObject.CompareTag("Asteroid"))
        {

            // Scales the resulting asteroid debris to be proportional with its size integer as referenced in the asteroid behavior script
            asteroidExplosion.transform.localScale = new Vector3(impactingObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize + 1,
                impactingObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize + 1, impactingObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize + 1);

            // Creates the explosions
            Instantiate(projectileExplosion, gameObject.transform.position, Quaternion.identity);
            Instantiate(asteroidExplosion, impactingObject.transform.position, Quaternion.identity);
            Instantiate(explosionModule, impactingObject.transform.position, Quaternion.identity);

            // Awards the player points as provided in the target asteroid's script
            lcReference.playerScore += impactingObject.gameObject.GetComponent<AsteroidBehavior>().pointsToAdd;

            // Causes the asteroid to fragment if it is not null
            if (impactingObject.gameObject.GetComponent<AsteroidMitosis>()) impactingObject.gameObject.GetComponent<AsteroidMitosis>().EjectFragments();

            // Destroys the asteroid
            Destroy(impactingObject.gameObject);

            // Marks the missile as having impacted the object
            missileHasImpacted = true;

        }
        // If the colliding object is a UFO (as appropriately tagged)
        else if (impactingObject.gameObject.CompareTag("Hostile Spacecraft"))
        {

            // Creates the explosions
            Instantiate(projectileExplosion, gameObject.transform.position, Quaternion.identity);
            Instantiate(hostileShipExplosion, impactingObject.transform.position, Quaternion.identity);
            Instantiate(explosionModule, impactingObject.transform.position, Quaternion.identity);

            // Awards the player points as provided in the UFO's script
            lcReference.playerScore += impactingObject.gameObject.GetComponent<UFOBehavior>().pointsWorth;

            // Destroys the UFO
            Destroy(impactingObject.gameObject);

            // Marks the missile as having impacted the object
            missileHasImpacted = true;

        }

    }

}
