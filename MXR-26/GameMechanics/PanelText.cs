/* Charlie Dye - ECT 4440 - 2026.03.28

This is the script for dynamic text in the cockpit */

using TMPro;
using UnityEngine;

public class PanelText : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the level counter.")] public LevelCounter lcReference;
    [Tooltip("The script attached to the list of power-ups.")] public PowerUpBank pubReference;
    [Tooltip("The script attached to player movement.")] public SpaceshipBehavior sbReference;
    [Tooltip("The script attached to the UFO spawning mechanics.")] public UFOSpawning ufosReference;
    [Tooltip("The script attached to the weapons system.")] public WeaponBehavior wbReference;

    [Header("Text Variables")]
    [Tooltip("The text displaying the usability status of the bullets.")] public TextMeshPro bulletText;
    [Tooltip("The text displaying the current cooldown of the bullets, if charging.")] public TextMeshPro bulletNumber;
    [Tooltip("The text displaying the usability status of the missiles.")] public TextMeshPro missileText;
    [Tooltip("The text displaying the current cooldown of the missiles, if charging.")] public TextMeshPro missileNumber;
    [Tooltip("The text displaying the vulnerability of the ship to exterior hazards.")] public TextMeshPro shipText;
    [Tooltip("The text displaying the amount of asteroids currently in the arena.")] public TextMeshPro asteroidText;
    [Tooltip("The text displaying whether a UFO is in the arena.")] public TextMeshPro ufoText;

    [Header("String Variables")]
    [Tooltip("The phrase denoting that a particular weapon is ready to use.")] public string readyMessage;
    [Tooltip("The phrase denoting that a particular weapon is charging.")] public string chargingMessage;
    [Tooltip("The phrase denoting that the ship is invincible.")] public string invincibleMessage;
    [Tooltip("The phrase denoting that the ship is shielded by the power-up.")] public string shieldedMessage;
    [Tooltip("The phrase denoting that the ship is vulnerable.")] public string vulnerableMessage;
    [Tooltip("The phrase denoting that no UFO is present.")] public string noNearbyShipMessage;
    [Tooltip("The phrase denoting that a UFO is present.")] public string nearbyShipMessage;


    [Header("Colors")]
    [Tooltip("The default color.")] public Color defaultColor;
    [Tooltip("The text color denoting all-clear.")] public Color clearColor;
    [Tooltip("The text color denoting caution.")] public Color cautionColor;
    [Tooltip("The text color denoting danger.")] public Color dangerColor;

    void Awake()
    {
        
        /* If any of the script references are left null, then this script will attempt to locate them by searching the scene;
        if that fails, then this script will self-destruct */
        if (lcReference == null)
        {

            if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
            else Destroy(this);

        }
        if (pubReference  == null)
        {

            if (FindAnyObjectByType<PowerUpBank>()) pubReference = FindFirstObjectByType<PowerUpBank>();
            else Destroy(this);

        }
        if (sbReference == null)
        {

            if (FindFirstObjectByType<SpaceshipBehavior>()) sbReference = FindFirstObjectByType<SpaceshipBehavior>();
            else Destroy(this);

        }
        if (ufosReference == null)
        {

            if (FindFirstObjectByType<UFOSpawning>()) ufosReference = FindFirstObjectByType<UFOSpawning>();
            else Destroy(this);

        }
        if (wbReference  == null)
        {

            if (FindFirstObjectByType<WeaponBehavior>()) wbReference = FindFirstObjectByType<WeaponBehavior>();
            else Destroy(this);

        }

    }

    void FixedUpdate()
    {
        
        // Logic concerning the bullet text
        if (wbReference.bulletsAreReady)
        {

            bulletText.text = readyMessage;
            bulletNumber.text = null;
            bulletText.color = clearColor;
            bulletNumber.color = clearColor;

        }
        else
        {

            bulletText.text = chargingMessage;
            if (wbReference.currentBulletCooldown >= 0f) bulletNumber.text = wbReference.currentBulletCooldown.ToString("F2");
            else bulletNumber.text = "0.00";
            bulletText.color = cautionColor;
            bulletNumber.color = cautionColor;

        }

        // Logic concerning the missile text
        if (wbReference.missileIsReady)
        {

            missileText.text = readyMessage;
            missileNumber.text = null;
            missileText.color = clearColor;
            missileNumber.color = clearColor;

        }
        else
        {

            missileText.text = chargingMessage;
            if (wbReference.currentMissileCooldown >= 0f) missileNumber.text = wbReference.currentMissileCooldown.ToString("F2");
            else missileNumber.text = "0.00";
            missileText.color = cautionColor;
            missileNumber.color = cautionColor;

        }

        // Logic concerning the protection level of the ship
        if (sbReference.isInvincible)
        {

            shipText.text = invincibleMessage;
            shipText.color = clearColor;

        }
        else if (!sbReference.isInvincible && pubReference.shCheck)
        {

            shipText.text = shieldedMessage;
            shipText.color = cautionColor;

        }
        else if (!sbReference.isInvincible && !pubReference.shCheck)
        {

            shipText.text = vulnerableMessage;
            shipText.color = dangerColor;

        }

        // Logic concerning the number displayed by the asteroid text
        asteroidText.text = lcReference.asteroidsRemaining.ToString();
        if (lcReference.asteroidsRemaining > 0) asteroidText.color = defaultColor;
        else asteroidText.color = clearColor;

        // Logic concerning any UFOs in the scene
        if (ufosReference.shipIsInArena)
        {

            ufoText.text = nearbyShipMessage;
            ufoText.color = dangerColor;

        }
        else
        {

            ufoText.text = noNearbyShipMessage;
            ufoText.color = clearColor;

        }

    }

}