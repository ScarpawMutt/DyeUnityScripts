/* Charlie Dye - ECT 4440 - 2026.01.22

This is the script for managing the spaceship's weapon systems */

using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponBehavior : MonoBehaviour
{

    [Header("Script and Input Action References")]
    [Tooltip("The script that references the master game controls.")] public GameControls gcReference;
    [Tooltip("The script that references the level mechanics.")] public LevelCounter lcReference;
    [Tooltip("The script attached to the player's power-up counter.")] public PowerUpBank pubReference;
    [Tooltip("The script that handles the spaceship's main controls.")] public SpaceshipBehavior sbReference;

    [Header("Input Actions")]
    [Tooltip("The keyboard binding to fire the ship's cannons.")] public InputAction launchBullets;
    [Tooltip("The keyboard binding to fire a missile.")] public InputAction launchMissile;

    [Header("Objects")]
    [Tooltip("The prefab to be spawned when the player fires bullets.")] public GameObject playerBullet;
    [Tooltip("The prefab to be spawned when the player fires a missile.")] public GameObject playerMissile;

    [Header("Transform Variables")]
    [Tooltip("The location where the left bullet in each volley spawns regardless of power-ups applied.")] public Transform portBulletSpawnLocationBasic;
    [Tooltip("The location where the right bullet in each volley spawns regardless of power-ups applied.")] public Transform starboardBulletSpawnLocationBasic;
    [Tooltip("The location where the left bullet in each volley spawns if the appropriate power-up is applied.")] public Transform portBulletSpawnLocationUpgraded;
    [Tooltip("The location where the right bullet in each volley spawns if the appropriate power-up is applied.")] public Transform starboardBulletSpawnLocationUpgraded;
    [Tooltip("The location where missiles spawn.")] public Transform missileSpawnLocation;

    [Header("Audio")]
    [Tooltip("The audio source to play when bullets fire.")] public AudioSource bulletFire;
    [Tooltip("The audio source to play when bullets fire.")] public AudioSource missileFire;
    [Tooltip("The audio source to play when the launch button in the cockpit is pressed.")] public AudioSource buttonClick;

    [Header("Float Variables")]
    [Tooltip("The cooldown before the player can fire bullets again.")] public float currentBulletCooldown;
    [Tooltip("The cooldown for bullets if the player has the appropriate buff applied.")] public float buffedBulletCooldown;
    [Tooltip("The cooldown before the player can launch a missile again.")] public float currentMissileCooldown;
    [Tooltip("The cooldown for bullets if the player has the appropriate buff applied.")] public float buffedMissileCooldown;
    [Tooltip("The amount of pitch that can be varied in each play of a sound effect.")] public float amountOfPitchVariance;
    private float originalBulletCooldown;
    private float originalMissileCooldown;
    private float originalBuffedBulletCooldown;
    private float originalBuffedMissileCooldown;

    [Header("Boolean Variables")]
    [Tooltip("Are the cannons ready to fire?")] public bool bulletsAreReady;
    [Tooltip("Is a missile ready to fire?")] public bool missileIsReady;

    void Awake()
    {

        // If the bullet or missile prefabs are left null, or if their spawn locations are left null, then this script will self-destruct
        if (playerBullet == null || playerMissile == null) Destroy(this);
        if (portBulletSpawnLocationBasic == null || starboardBulletSpawnLocationBasic == null || missileSpawnLocation == null) Destroy(this);

    }

    void Start()
    {

        // If the float variables have improper values, then this will correct them
        if (currentBulletCooldown == 0f) currentBulletCooldown = 0.25f;
        else if (currentBulletCooldown < 0f) currentBulletCooldown *= -1f;
        if (currentMissileCooldown == 0f) currentMissileCooldown = 30f;
        else if (currentMissileCooldown < 0f) currentMissileCooldown *= -1f;
        if (buffedBulletCooldown >= currentBulletCooldown) buffedBulletCooldown = currentBulletCooldown - 0.1f;
        if (buffedMissileCooldown >= currentMissileCooldown) buffedMissileCooldown = currentMissileCooldown - 0.1f;
        if (amountOfPitchVariance < 0f) amountOfPitchVariance *= -1f;

        // Sets the private floats to their respective public values
        originalBulletCooldown = currentBulletCooldown;
        originalMissileCooldown = currentMissileCooldown;
        originalBuffedBulletCooldown = buffedBulletCooldown;
        originalBuffedMissileCooldown = buffedMissileCooldown;

        // Enables the input actions for inputs by default
        launchBullets.Enable();
        launchMissile.Enable();

        // Sets the public Boolean values to true if not already, allowing for immediate usage of the weapons
        if (!bulletsAreReady) bulletsAreReady = true;
        if (!missileIsReady) missileIsReady = true;

    }

    void FixedUpdate()
    {

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

        // If the bullet or missile systems must start a cooldown (controlled by the public Booleans), then their methods to do will will activate
        if (!bulletsAreReady) CannonsCooldown();
        if (!missileIsReady) MissileCooldown();

    }

    private void BindingControl(bool conventional)
    {

        if (conventional)
        {

            // If a given binding is pressed, it will activate its associated method below
            if (launchBullets.IsPressed() && bulletsAreReady) LaunchBullets();
            if (launchMissile.IsPressed() && missileIsReady) LaunchMissile();

        }
        else
        {

            // If a VR binding is pressed, it will activate its associated method below
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0f && bulletsAreReady) LaunchBullets();
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0f && missileIsReady) LaunchMissile();

        }

    }

    public void LaunchBullets()
    {

        // Spawns two bullets; one appear at each spawn spot
        Instantiate(playerBullet, new Vector3(portBulletSpawnLocationBasic.position.x, portBulletSpawnLocationBasic.position.y, portBulletSpawnLocationBasic.position.z), portBulletSpawnLocationBasic.rotation);
        Instantiate(playerBullet, new Vector3(starboardBulletSpawnLocationBasic.position.x, starboardBulletSpawnLocationBasic.position.y, starboardBulletSpawnLocationBasic.position.z), starboardBulletSpawnLocationBasic.rotation);

        // If the appropriate buff is active
        if (pubReference.qcCheck)
        {

            // Two more will spawn per volley at their own spawn spots
            Instantiate(playerBullet, new Vector3(portBulletSpawnLocationUpgraded.position.x, portBulletSpawnLocationUpgraded.position.y, portBulletSpawnLocationUpgraded.position.z), portBulletSpawnLocationUpgraded.rotation);
            Instantiate(playerBullet, new Vector3(starboardBulletSpawnLocationUpgraded.position.x, starboardBulletSpawnLocationUpgraded.position.y, starboardBulletSpawnLocationUpgraded.position.z), starboardBulletSpawnLocationUpgraded.rotation);

            // Increments the launched bullet counter by four
            lcReference.numberOfBulletsFired += 4;

        }
        // Otherwise, the launched bullet counter is increased by two
        else lcReference.numberOfBulletsFired += 2;

        // Sets the Boolean value to false, automatically initiating the method for the cooldown
        bulletsAreReady = false;

        // Plays the sound of bullets launching with a randomized pitch value
        bulletFire.pitch = UnityEngine.Random.Range(1f - amountOfPitchVariance, 1f + amountOfPitchVariance);
        bulletFire.Play();

    }

    public void LaunchMissile()
    {

        // Spawns a missile
        Instantiate(playerMissile, new Vector3(missileSpawnLocation.position.x, missileSpawnLocation.position.y, missileSpawnLocation.position.z), missileSpawnLocation.rotation);

        // Increments the launched missile counter by one
        lcReference.numberOfMissilesFired++;

        // Sets the Boolean value to false, automatically initiating the method for the cooldown
        missileIsReady = false;

        // Plays the sound of a missile launching with a randomized pitch value along with the button press
        missileFire.pitch = UnityEngine.Random.Range(1f - amountOfPitchVariance, 1f + amountOfPitchVariance);
        buttonClick.pitch = UnityEngine.Random.Range(1f - amountOfPitchVariance, 1f + amountOfPitchVariance);
        missileFire.Play();
        buttonClick.Play();

    }

    private void CannonsCooldown()
    {

        // Whether or not the player has the appropriate buff applied
        if (pubReference.rfCheck)
        {

            // Counts down in real time to zero, beginning at the associated public float
            if (buffedBulletCooldown > 0f) buffedBulletCooldown -= Time.fixedDeltaTime;
            // Otherwise, the missile system is reset
            else
            {

                // Sets the current cooldown time back to its original value when loaded (as stored by its private counterpart)
                buffedBulletCooldown = originalBuffedBulletCooldown;

                // Sets the Boolean back to true, allowing for a reload
                bulletsAreReady = true;

            }

        }
        else
        {

            // Counts down in real time to zero, beginning at the associated public float
            if (currentBulletCooldown > 0f) currentBulletCooldown -= Time.fixedDeltaTime;
            // Otherwise, the cannon system is reset
            else
            {

                // Sets the current cooldown time back to its original value when loaded (as stored by its private counterpart)
                currentBulletCooldown = originalBulletCooldown;

                // Sets the Boolean back to true, allowing for a reload
                bulletsAreReady = true;

            }

        }

    }

    private void MissileCooldown()
    {

        // Whether or not the player has the appropriate buff applied
        if (pubReference.bmCheck)
        {

            // Counts down in real time to zero, beginning at the associated public float
            if (buffedMissileCooldown > 0f) buffedMissileCooldown -= Time.fixedDeltaTime;
            // Otherwise, the missile system is reset
            else
            {

                // Sets the current cooldown time back to its original value when loaded (as stored by its private counterpart)
                buffedMissileCooldown = originalBuffedMissileCooldown;

                // Sets the Boolean back to true, allowing for a reload
                missileIsReady = true;

            }

        }
        else
        {

            // Counts down in real time to zero, beginning at the associated public float
            if (currentMissileCooldown > 0f) currentMissileCooldown -= Time.fixedDeltaTime;
            // Otherwise, the missile system is reset
            else
            {

                // Sets the current cooldown time back to its original value when loaded (as stored by its private counterpart)
                currentMissileCooldown = originalMissileCooldown;

                // Sets the Boolean back to true, allowing for a reload
                missileIsReady = true;

            }

        }

    }

}
