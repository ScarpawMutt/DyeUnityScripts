/* Charlie Dye - ECT 4440 - 2026.04.28

This is the script for the shields protecting the player */

using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the level controls.")] public LevelCounter lcReference;
    [Tooltip("The script attached to the player's power-up counter.")] public PowerUpBank pubReference;
    [Tooltip("The script attached to the player movement module.")] public SpaceshipBehavior sbReference;
    [Tooltip("The script attached to the player's weapon systems.")] public WeaponBehavior wbReference;

    [Header("Objects")]
    [Tooltip("The shield that appears when the player has the appropriate power-up applied.")] public GameObject shieldNormal;
    [Tooltip("The shield that appears when the player is invincible.")] public GameObject shieldInvincible;

    [Header("Float Variable")]
    [Tooltip("The magnitude of the repelling force created by a collision with the shield.")] public float repellingForce;

    [Header("Special Effects")]
    [Tooltip("The explosion that occurs when the shield touches an asteroid.")] public GameObject asteroidExplosion;
    [Tooltip("The explosion that occurs when the shield touches a UFO.")] public GameObject hostileShipExplosion;
    [Tooltip("The explosion that occurs when the shield touches a hostile bullet.")] public GameObject hostileProjectileExplosion;
    [Tooltip("The prefab that creates the explosion audio.")] public GameObject explosionModule;

    // Collider variable
    private Collider shieldBarrier;

    private void Awake()
    {
        
        /* If any of the public script variables are left null, then this script will attempt to locate them by searching the scene by class;
        if this fails, then this script will self-destruct */
        if (lcReference == null)
        {

            if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
            else Destroy(this);

        }
        if (pubReference  == null)
        {

            if (FindFirstObjectByType<PowerUpBank>()) pubReference = FindFirstObjectByType<PowerUpBank>();
            else Destroy(this);

        }
        if (sbReference == null)
        {

            if (FindFirstObjectByType<SpaceshipBehavior>()) sbReference = FindFirstObjectByType<SpaceshipBehavior>();
            else Destroy(this);

        }
        if (wbReference == null)
        {

            if (FindFirstObjectByType<WeaponBehavior>()) wbReference = FindFirstObjectByType<WeaponBehavior>();
            else Destroy(this);

        }

    }

    void Start()
    {

        /* If the parent object contains a collider, then this script will fetch it as the reference to the private collider variable;
        otherwise, this script will self-destruct */
        if (gameObject.GetComponent<Collider>()) shieldBarrier = gameObject.GetComponent<Collider>();
        else Destroy(this);

        // If the float has an unworkable value, then this will correct it
        if (repellingForce == 0f) repellingForce = 1f;
        else if (repellingForce < 0f) repellingForce *= -1f;

    }

    void FixedUpdate()
    {

        // Follows the player
        gameObject.transform.SetPositionAndRotation(sbReference.gameObject.transform.position, sbReference.gameObject.transform.rotation);

        // If the player has the shield power-up applied and the normal shield is not deployed, then it will do so
        if (pubReference.shCheck && !shieldNormal.activeInHierarchy) shieldNormal.SetActive(true);
        // If the player is no longer buffed but the normal shield is still active, then it will deactivate
        else if (!pubReference.shCheck && shieldNormal.activeInHierarchy) shieldNormal.SetActive(false);

        // If the player is invincible and the invincibility shield has not been deployed, then it will do so
        if (sbReference.isInvincible && !shieldInvincible.activeInHierarchy) shieldInvincible.SetActive(true);
        // If the player is no longer invincible but the invincibility shield is still active, then it will deactivate
        else if (!sbReference.isInvincible && shieldInvincible.activeInHierarchy) shieldInvincible.SetActive(false);

        /* If either of the above Booleans are true and the actual shield collider is not yet active, then it does so;
        if neither are true and it is still active, then it deactivates */
        if ((sbReference.isInvincible || pubReference.shCheck) && !shieldBarrier.enabled) shieldBarrier.enabled = true;
        else if (!sbReference.isInvincible && !pubReference.shCheck && shieldBarrier.enabled) shieldBarrier.enabled = false;

    }

    // If the shield touches a harmful object while active
    void OnCollisionEnter(Collision impactingObject)
    {

        // If the colliding object is an asteroid
        if (impactingObject.gameObject.CompareTag("Asteroid"))
        {

            // Creates the explosion effects
            Instantiate(asteroidExplosion, impactingObject.transform.position, Quaternion.identity);
            Instantiate(explosionModule, impactingObject.transform.position, Quaternion.identity);

            // Awards the player points as provided in the target asteroid's script
            lcReference.playerScore += impactingObject.gameObject.GetComponent<AsteroidBehavior>().pointsToAdd;

            // Causes the asteroid to fragment if it is larger than the smallest size
            if (impactingObject.gameObject.GetComponent<AsteroidBehavior>().asteroidSize > 0) impactingObject.gameObject.GetComponent<AsteroidMitosis>().EjectFragments();

            // Propels the player away from the collision
            CreateBlowback(impactingObject.transform.position, 1f);

            // Destroys the asteroid
            Destroy(impactingObject.gameObject);

            // Sets the appropriate Booleans in other scripts to proper values to allow for another deployment
            pubReference.shCheck = false;

        }
        // If the colliding object is a harmful bullet (i.e., one fired from a UFO)
        else if (impactingObject.gameObject.CompareTag("Hostile Bullet"))
        {

            // Creates the explosion effects
            Instantiate(hostileProjectileExplosion, impactingObject.transform.position, Quaternion.identity);
            Instantiate(explosionModule, impactingObject.transform.position, Quaternion.identity);

            // Propels the player away from the collision
            CreateBlowback(impactingObject.transform.position, 0.25f);

            // Destroys the bullet
            Destroy(impactingObject.gameObject);

            // Sets the appropriate Booleans in other scripts to proper values to allow for another deployment
            pubReference.shCheck = false;

        }
        // If the colliding object is a UFO
        else if (impactingObject.gameObject.CompareTag("Hostile Spacecraft"))
        {

            // Creates the explosion effects
            Instantiate(hostileShipExplosion, impactingObject.transform.position, Quaternion.identity);
            Instantiate(explosionModule, impactingObject.transform.position, Quaternion.identity);

            // Awards the player points as provided in the target asteroid's script
            lcReference.playerScore += impactingObject.gameObject.GetComponent<UFOBehavior>().pointsWorth;

            // Propels the player away from the collision
            CreateBlowback(impactingObject.transform.position, 0.5f);

            // Destroys the UFO
            Destroy(impactingObject.gameObject);

            // Sets the appropriate Booleans in other scripts to proper values to allow for another deployment
            pubReference.shCheck = false;

        }

    }

    private void CreateBlowback(Vector3 impactingLocation, float impactMultiplier)
    {

        // Introduces local vector-3 variables that store the locations of the player and the object they have collided with
        Vector3 playerLocation = sbReference.cockpitObject.transform.position;
        Vector3 repellingDirection = playerLocation - impactingLocation;

        // Normalizes the resulting vector
        repellingDirection.Normalize();

        // Creates the repelling force
        sbReference.cockpitObject.AddRelativeForce(repellingForce * impactMultiplier * Time.fixedDeltaTime * repellingDirection);

    }

}
