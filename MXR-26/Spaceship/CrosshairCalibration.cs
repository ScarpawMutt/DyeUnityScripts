/* Charlie Dye - ECT 4440 - 2026.04.07

This is the script for automatic adjustment of the crosshair height */

using UnityEngine;

public class CrosshairCalibration : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script that references the master game controls.")] public GameControls gcReference;

    [Header("Camera")]
    [Tooltip("The player's POV camera.")] public Camera playerCamera;

    [Header("Crosshair Object")]
    [Tooltip("The object representing the 2-D crosshair texture.")] public GameObject crosshairObject;

    [Header("Float Variable")]
    [Tooltip("The speed at which the crosshair calibrates.")] public float adjustmentRate;

    // Vector-3 variable
    private Vector3 targetHeight;

    void Awake()
    {

        // If the crosshair game object and/or camera is/are left null, then this script will self-destruct
        if (crosshairObject == null || playerCamera == null) Destroy(this);

        /* If the public script reference is left null, then this script will attempt to locate it;
        if that fails, then this script will self-destruct */
        if (gcReference == null)
        {

            if (FindFirstObjectByType<GameControls>()) gcReference = gameObject.AddComponent<GameControls>();
            else Destroy(this);

        }

    }

    void Start()
    {

        // If the floats have bad values, then this will correct them
        if (adjustmentRate == 0f) adjustmentRate = 0.1f;
        else if (adjustmentRate < 0f) adjustmentRate *= -1f;

        // If the game is using conventional keyboard controls, then this script will deactivate
        if (!gcReference.useXRControls) this.enabled = false;

    }

    void FixedUpdate()
    {
        
        // If the crosshair's Y-value (height) does not equal that of the camera
        if (crosshairObject.transform.position.y != playerCamera.transform.position.y)
        {

            // The vector system updates, using the player camera's height as the Y-value
            targetHeight = new(crosshairObject.transform.position.x, playerCamera.transform.position.y, crosshairObject.transform.position.z);

            // Moves the crosshair to the desired height
            crosshairObject.transform.position = Vector3.Lerp(crosshairObject.transform.position, targetHeight, adjustmentRate);

        }

    }

}
