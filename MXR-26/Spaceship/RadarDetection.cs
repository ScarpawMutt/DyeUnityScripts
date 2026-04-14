/* Charlie Dye - ECT 4440 - 2026.02.20

This is the script for detection of obstacles by the ship's radar */

using UnityEngine;

public class RadarDetection : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the primary radar mechanism. These scripts should always be mated.")] public RadarBehavior rbReference;

    void Awake()
    {
        
        /* If the radar behavior reference is left null, then this script will attempt to locate it;
        if that fails, then this script will self-destruct */
        if (rbReference == null)
        {

            if (FindFirstObjectByType<RadarBehavior>()) rbReference = FindFirstObjectByType<RadarBehavior>();
            else Destroy(this);

        }

    }

    void OnTriggerEnter(Collider signature)
    {

        // If the radar signature is an asteroid
        if (signature.CompareTag("Asteroid"))
        {

            // Plays a blipping noise by accessing the object's script with the proper method
            signature.GetComponent<AsteroidBehavior>().CreateRadarBlip();

        }
        // If the radar signature is a UFO
        else if (signature.CompareTag("Hostile Spacecraft"))
        {

            // Plays a blipping noise by accessing the object's script with the proper method
            signature.GetComponent<UFOBehavior>().CreateRadarBlip();

        }
        // If the radar signature is a power-up
        else if (signature.CompareTag("Power-Up"))
        {

            // Plays a blipping noise by accessing the object's script with the proper method
            signature.GetComponent<PowerUpBehavior>().CreateRadarBlip();

        }

    }

}
