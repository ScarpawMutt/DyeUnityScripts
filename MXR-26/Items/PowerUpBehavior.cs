/* Charlie Dye - ECT 4440 - 2026.02.05

This is the script for power-ups in general */

using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script containing the power-up enum.")] public PowerUpList pulReference;
    private LevelCounter lcReference;
    private PowerUpBank pubReference;

    [Header("Audio")]
    [Tooltip("The sound created when the player's radar hits this object.")] public AudioSource radarBlip;

    [Header("Numerical Variables")]
    [Tooltip("The rotation rate of this object.")] public float rotationRate;
    [Tooltip("The amount of points that collecting this object adds to the player's score.")] public int pointsThisObjectAdds;

    void Awake()
    {

        /* Attempts to automatically load the private script variable by searching the scene for the objects they are attached to;
        if this fails, then this script will self-destruct */
        if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
        else Destroy(this);
        if (FindFirstObjectByType<SpaceshipBehavior>()) pubReference = FindFirstObjectByType<PowerUpBank>();
        else Destroy(this);

    }

    void OnEnable()
    {

        // If the integer is configured to subtract points, then this will correct that
        if (pointsThisObjectAdds < 0) pointsThisObjectAdds *= -1;

    }

    void FixedUpdate()
    {

        // Rotates the object along its Z-axis
        gameObject.transform.Rotate(Time.fixedDeltaTime * rotationRate * Vector3.up);

    }

    void OnTriggerEnter(Collider player)
    {
        
        // If the triggering object is tagged correctly and contains a collider component
        if (player.CompareTag("Player") && player.GetComponent<Collider>())
        {

            /* Depending on which enum value this object is marked by, collecting this will grant a different effect;
            with the exception of the extra life buff, all information is passed on into PowerUpBank.cs */
            if (pulReference == PowerUpList.QuadCannons) pubReference.qcCheck = true;
            else if (pulReference == PowerUpList.RapidFire) pubReference.rfCheck = true;
            else if (pulReference == PowerUpList.ClearSteer) pubReference.csCheck = true;
            else if (pulReference == PowerUpList.FasterBullets) pubReference.fbCheck = true;
            else if (pulReference == PowerUpList.BetterMissiles) pubReference.bmCheck = true;
            else if (pulReference == PowerUpList.PowerfulThrusters) pubReference.ptCheck = true;
            else if (pulReference == PowerUpList.Shield) pubReference.shCheck = true;
            else if (pulReference == PowerUpList.ExtraLife) lcReference.playerLives++;

            // Adds the given point integer value to the player's score
            lcReference.playerScore += pointsThisObjectAdds;

            // Increments a value of one to the integer counter in the power up bank
            lcReference.powerUpsCollected++;

            // Destroys this object
            Destroy(gameObject);

        }

    }

    public void CreateRadarBlip()
    {

        // If the ship's radar system hits this object (i.e., detects it with a trigger collider), then this object will play a spatialized blip
        radarBlip.Play();

    }

}
