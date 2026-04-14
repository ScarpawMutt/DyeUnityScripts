/* Charlie Dye, PACE Team - 2025.11.07

This is the script that creates the illusion of quaking */

using UnityEngine;

public class CameraQuake : MonoBehaviour
{

    [Header("Player Variable")]
    [Tooltip("The avatar's transform on the player's XR Rig.")] public Transform playerPOV;

    [Header("Float Variables")]
    [Tooltip("The lateral severity of the quaking effect.")] public float quakingIntensity;
    [Tooltip("The intensity of the tilting effect of the camera when quaking occurs.")] public float tiltingIntensity;
    [Tooltip("The amount interpolated by each method call when the camera is rotated off its rest orientation.")] public float slerpAmount;

    [Header("Boolean Variable")]
    [Tooltip("Can the camera quake?")] public bool canQuake = false;
    private bool quakeHasStarted;
    private bool quakeHasStopped;

    void Start()
    {

        // If the camera variable is left null, the script will self-destruct
        if (playerPOV == null) Destroy(this);

    }

    void FixedUpdate()
    {

        // If the quake must start
        if (canQuake) ShakeCamera();

        // Checks the status of the quaking effect
        StatusCheck();
        
    }

    private void StatusCheck()
    {

        // If the quaking effect is active
        if (canQuake)
        {

            // If the start kill Boolean has not switched values
            if (!quakeHasStarted)
            {

                // Inverts the values of the kill Booleans
                quakeHasStarted = true;
                quakeHasStopped = false;

            }

        }
        // If the quaking effect is inactive
        else
        {

            // If the stop kill Boolean has not switched values
            if (!quakeHasStopped)
            {

                // Resets the camera
                ResetQuake();

                // Inverts the values of the kill Booleans
                quakeHasStarted = false;
                quakeHasStopped = true;

            }

        }

    }

    private void ShakeCamera()
    {

        // Introduces local variables that store random values for the camera's position and rotation
        Vector3 randomPosition = new(0f + Random.Range(-quakingIntensity, quakingIntensity), 0f + Random.Range(-quakingIntensity, quakingIntensity), 0f);
        Quaternion randomRotation = Quaternion.Slerp(Quaternion.identity, new Quaternion(playerPOV.localRotation.x, playerPOV.localRotation.y,
            playerPOV.localRotation.z + Random.Range(-tiltingIntensity, tiltingIntensity), playerPOV.localRotation.w), slerpAmount);

        // Applies those variables to the camera
        playerPOV.localPosition = randomPosition;
        playerPOV.localRotation = randomRotation;

    }

    public void ResetQuake()
    {

        // Places the camera back where it should be
        playerPOV.localPosition = Vector3.zero;
        playerPOV.localRotation = Quaternion.identity;

    }

}
