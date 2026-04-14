/* Charlie Dye - ECT 4440 - 2026.01.25

This is the script for controlling the spawned bullet projectiles */

using UnityEngine;

public class BulletBehavior : MonoBehaviour
{

    [Header("Special Effects")]
    [Tooltip("The explosion that occurs when the bullet hits an asteroid.")] public GameObject asteroidExplosion;
    [Tooltip("The explosion that occurs when the missile hits a UFO, created by the object itself.")] public GameObject hostileShipExplosion;
    [Tooltip("The explosion that occurs when the missile hits an object, created by this projectile, if friendly.")] public GameObject projectileExplosionFriendly;
    [Tooltip("The explosion that occurs when the missile hits an object, created by this projectile, if hostile.")] public GameObject projectileExplosionHostile;
    [Tooltip("The prefab that creates the explosion audio.")] public GameObject explosionModule;

    [Header("Float Variables")]
    [Tooltip("The speed of this bullet once fired.")] public float bulletSpeed;
    [Tooltip("The value the speed of the bullet is multiplied by if the appropriate buff is applied.")] public float speedMultiplier;
    [Tooltip("The maximum distance this bullet can travel before it must despawn.")] public float deathBound;
    private float speedInsert;

    [Header("Boolean Variable")]
    [Tooltip("Is this bullet friendly (fired by the player; true) or hostile (fired by a UFO; false)?")] public bool isFriendly;

    // Script reference
    private LevelCounter lcReference;
    private PowerUpBank pubReference;
    private SpaceshipBehavior sbReference;

    void Awake()
    {

        /* Scans the scene for objects with the appropriate scripts (this should always return true in practice);
        as long as it does, it will assign the private script reference with what it finds */
        if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
        if (FindFirstObjectByType<PowerUpBank>()) pubReference = FindFirstObjectByType<PowerUpBank>();
        if (FindFirstObjectByType<SpaceshipBehavior>()) sbReference = FindFirstObjectByType<SpaceshipBehavior>();

    }

    void OnEnable()
    {

        // If the bullet is friendly, then it ignores collision with objects that belong in the layer for either the player (3) or other projectiles (6)
        if (gameObject.CompareTag("Friendly Bullet"))
        {

            Physics.IgnoreLayerCollision(3, 6);
            Physics.IgnoreLayerCollision(6, 6);

        }

        // If the player is powered up appropriately and not destroyed, then the bullet's speed will be multiplied
        if (pubReference != null && pubReference.fbCheck) speedInsert = speedMultiplier;
        else speedInsert = 1.0f;

        // If the float variables are configured incorrectly on the parent prefab, then this will fix them
        if (bulletSpeed == 0f) bulletSpeed = 10000f;
        else if (bulletSpeed < 0f) bulletSpeed *= -1f;
        if (speedMultiplier <= 1.0f) speedMultiplier = 2.0f;
        if (deathBound == 0f) deathBound = 600f;
        else if (deathBound < 0f) deathBound *= -1f;

        // Causes the bullet to move forward in a straight line relative to its local rotation at a constant rate; the velocity of the ship is preserved
        if (gameObject.GetComponent<Rigidbody>()) gameObject.GetComponent<Rigidbody>().AddRelativeForce
                (bulletSpeed * speedInsert * Time.fixedDeltaTime * Vector3.forward + sbReference.cockpitObject.linearVelocity);

    }

    void FixedUpdate()
    {

        // If the bullet is too far away, then it will despawn
        if (gameObject.transform.position.x > deathBound || gameObject.transform.position.x < -deathBound ||
            gameObject.transform.position.z > deathBound || gameObject.transform.position.z < -deathBound) Destroy(gameObject);

    }

    void OnCollisionEnter(Collision impactedObject)
    {

        // If the bullet is friendly (i.e., fired from the player)
        if (isFriendly)
        {

            // If the colliding object is an asteroid (as correctly tagged)
            if (impactedObject.gameObject.CompareTag("Asteroid"))
            {

                // Scales the resulting asteroid debris to be proportional with its size integer as referenced in the asteroid behavior script
                asteroidExplosion.transform.localScale = new Vector3(impactedObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize + 1,
                    impactedObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize + 1, impactedObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize + 1);

                // Creates the explosions
                Instantiate(asteroidExplosion, impactedObject.transform.position, Quaternion.identity);
                Instantiate(projectileExplosionFriendly, gameObject.transform.position, Quaternion.identity);
                Instantiate(explosionModule, impactedObject.transform.position, Quaternion.identity);

                // Awards the player points as provided in the target asteroid's script
                lcReference.playerScore += impactedObject.gameObject.GetComponent<AsteroidBehavior>().pointsToAdd;

                // Increments the number of asteroids shot down by one
                lcReference.asteroidsShotDown++;

                // Causes the asteroid to fragment if it is larger than the smallest size
                if (impactedObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize > 0) impactedObject.gameObject.GetComponent<AsteroidMitosis>().EjectFragments();

                // Destroys the asteroid
                Destroy(impactedObject.gameObject);

                // Destroys this bullet (object)
                Destroy(gameObject);

            }
            // If the colliding object is a UFO (as correctly tagged)
            else if (impactedObject.gameObject.CompareTag("Hostile Spacecraft"))
            {

                // Creates the explosion
                Instantiate(projectileExplosionFriendly, gameObject.transform.position, Quaternion.identity);
                Instantiate(hostileShipExplosion, impactedObject.transform.position, Quaternion.identity);
                Instantiate(explosionModule, impactedObject.transform.position, Quaternion.identity);

                // Appends the UFO's points value to the player's score
                lcReference.playerScore += impactedObject.gameObject.GetComponent<UFOBehavior>().pointsWorth;

                // Increments the number of UFOs shot down by one
                lcReference.hostileShipsShotDown++;

                // Destroys the UFO
                Destroy(impactedObject.gameObject);

                // Destroys this bullet (object)
                Destroy(gameObject);

            }

        }
        // If the bullet is hostile (i.e., fired from a UFO)
        else
        {

            // If the colliding object is a bullet fired by the player (as correctly tagged)
            if (impactedObject.gameObject.CompareTag("Friendly Bullet"))
            {

                // Creates the explosions
                Instantiate(projectileExplosionFriendly, impactedObject.transform.position, Quaternion.identity);
                Instantiate(projectileExplosionHostile, gameObject.transform.position, Quaternion.identity);
                Instantiate(explosionModule, impactedObject.transform.position, Quaternion.identity);

                // Destroys both the impacted friendly bullet and this own bullet (i.e., they cancel each other out)
                Destroy(impactedObject.gameObject);
                Destroy(gameObject);

            }
            // If the colliding object is a missile
            else if (impactedObject.gameObject.CompareTag("Missile"))
            {

                // Creates the explosion
                Instantiate(projectileExplosionHostile, gameObject.transform.position, Quaternion.identity);
                Instantiate(explosionModule, impactedObject.transform.position, Quaternion.identity);

                // Marks the missile as falsely having impacted its target
                impactedObject.gameObject.GetComponent<MissileBehavior>().missileHasImpacted = true;

            }
            // If the colliding object is the player's ship (as correctly tagged)
            else if (impactedObject.gameObject.CompareTag("Player"))
            {

                // Creates the explosion
                Instantiate(projectileExplosionHostile, gameObject.transform.position, Quaternion.identity);

                // If the player is not invincible, then the player is destroyed by accessing the script that is always attached to the player
                if (!impactedObject.gameObject.GetComponent<SpaceshipBehavior>().isInvincible) impactedObject.gameObject.GetComponent<SpaceshipBehavior>().DestroyPlayer();

                // Destroys this bullet (object)
                Destroy(gameObject);

            }
            // If the colliding object is an asteroid (as correctly tagged)
            else if (impactedObject.gameObject.CompareTag("Asteroid"))
            {

                // Creates the explosions
                Instantiate(asteroidExplosion, impactedObject.transform.position, Quaternion.identity);
                Instantiate(projectileExplosionHostile, gameObject.transform.position, Quaternion.identity);
                Instantiate(explosionModule, impactedObject.transform.position, Quaternion.identity);

                // Causes the asteroid to fragment if it is larger than the smallest size
                if (impactedObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize > 0) impactedObject.gameObject.GetComponent<AsteroidMitosis>().EjectFragments();

                // Destroys the asteroid
                Destroy(impactedObject.gameObject);

                // Destroys this bullet (object)
                Destroy(gameObject);

            }

        }

    }

}
