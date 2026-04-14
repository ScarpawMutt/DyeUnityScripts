/* Charlie Dye - ECT 4440 - 2026.02.19

This is the script for primary radar action */

using UnityEngine;

public class RadarBehavior : MonoBehaviour
{

    [Header("Objects")]
    [Tooltip("The player's spaceship.")] public GameObject playerShip;
    [Tooltip("The transform that the ray is a direct child of.")] public GameObject orientationTransform;
    [Tooltip("The physical object acts as the radar scanner.")] public GameObject radarRay;

    [Header("Float Variables")]
    [Tooltip("How fast the radar rotates around the ship.")] public float rotationRate;
    [Tooltip("The length of one face of the play area.")] public float arenaLength;
    [Tooltip("The radius of the radar ray.")] public float rayRadius;

    private void Awake()
    {

        // If the spaceship or ray objects are left null, then this script will self-destruct
        if (playerShip == null || radarRay == null) Destroy(this);

    }

    void Start()
    {

        // If certain variables have improper values, then this will correct them
        if (rotationRate == 0f) rotationRate = 1f;
        else if (rotationRate < 0f) rotationRate *= -1f;
        if (arenaLength == 0f) arenaLength = 500f;
        else if (arenaLength < 0f) arenaLength *= -1f;
        if (rayRadius == 0f) rayRadius = 0.1f;
        else if (rayRadius < 0f) rayRadius *= -1f;

        // Sets the ray's transforms to the proper values as entered by the player
        radarRay.transform.localScale = new Vector3(rayRadius, arenaLength, rayRadius);
        radarRay.transform.localPosition = new Vector3(0f, 0f, arenaLength);

    }

    void FixedUpdate()
    {

        // Causes the radar's origin to follow the player as a separate object
        FollowPlayer();

        // Rotates the radar ray
        RotateRadar();

        // If the player is inactive (i.e., destroyed), then the radar will deactivate with it
        if (!playerShip.activeInHierarchy)
        {

            orientationTransform.SetActive(false);
            radarRay.SetActive(false);

        }
        else
        {

            orientationTransform.SetActive(true);
            radarRay.SetActive(true);

        }

    }

    private void FollowPlayer()
    {

        // Follows the player's position and rotation
        gameObject.transform.SetPositionAndRotation(playerShip.transform.position, playerShip.transform.rotation);

    }

    private void RotateRadar()
    {

        // Rotates the ray around the root (point of origin) by the given speed and in real time
        orientationTransform.transform.Rotate(rotationRate * Time.fixedDeltaTime * Vector3.up);

    }

}
