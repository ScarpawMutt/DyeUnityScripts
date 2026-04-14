/* Charlie Dye - ECT 4440 - 2026.02.03

This is the script for the spaceship death animation */

using UnityEngine;

public class DeathSequence : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the master level mechanics.")] public LevelCounter lcReference;
    [Tooltip("The script attached to the postmortem screen.")] public Postmortem pmReference;
    [Tooltip("The script attached to the text in the player UI.")] public TMPBehavior tmpbReference;

    [Header("Objects")]
    [Tooltip("The player camera.")] public GameObject playerCamera;
    [Tooltip("The explosion effect that appears when the player dies.")] public GameObject playerExplosion;
    [Tooltip("The randomized explosion audio.")] public GameObject explosionModule;
    private GameObject originalCameraParent;

    [Header("Vector Variables")]
    [Tooltip("The offset position that the death camera appears at when the player dies.")] public Vector3 cameraOffset;
    [HideInInspector] public Vector3 deathLocation;
    private Vector3 playerHeight;
    private Vector3 originalStartingCoordinates;

    [Header("Float Variables")]
    [Tooltip("The speed the camera zooms out at.")] public float zoomSpeed;
    [Tooltip("The duration of the zooming animation. Do not set this higher than the overall death duration.")] public float durationOfAnimation;
    [Tooltip("The overall duration of the death animation.")] public float durationOfDeath;
    [Tooltip("The duration of the \"game over\" screen.")] public float durationOfGameOver;
    private float originalAnimationLength;
    private float originalDeathLength;

    [Header("Boolean Variable")]
    [Tooltip("Is the player currently despawned and dead?")] public bool playerIsDead;
    [HideInInspector] public bool outOfLives = false;
    [HideInInspector] public bool cueStats = false;
    private bool canZoomOut = false;

    void Awake()
    {

        /* If either of the public script variables are left null, then this script will attempt to locate them by searching in the scene;
        if that fails, then this script will self-destruct */
        if (lcReference == null)
        {

            if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
            else Destroy(this);

        }
        if (pmReference  == null)
        {

            if (FindFirstObjectByType<Postmortem>()) pmReference = FindFirstObjectByType<Postmortem>();
            else Destroy(this);

        }
        if (tmpbReference == null)
        {

            if (FindFirstObjectByType<TMPBehavior>()) tmpbReference = FindFirstObjectByType<TMPBehavior>();
            else Destroy(this);

        }

        // If the camera object is left null, then this script will self-destruct
        if (playerCamera == null) Destroy(this);

    }

    void Start()
    {

        // Sets the private game object to the spaceship (i.e., the object with the player controller script)
        originalCameraParent = FindFirstObjectByType<SpaceshipBehavior>().gameObject;

        // Sets the player height as the offset of the camera, relative to the spaceship's center, as loaded in
        playerHeight = playerCamera.transform.localPosition;

        // Records the initial starting coordinates
        originalStartingCoordinates = originalCameraParent.transform.position;

        // Records the values of the public duration floats, copying them onto their private counterparts
        originalAnimationLength = durationOfAnimation;
        originalDeathLength = durationOfDeath;

        // If the floats have unworkable values, then this will correct them
        if (zoomSpeed == 0f) zoomSpeed = 1f;
        else if (zoomSpeed < 0f) zoomSpeed *= -1f;
        if (durationOfDeath == 0f) durationOfDeath = 10f;
        else if (durationOfDeath < 0f) durationOfDeath *= -1f;
        if (durationOfAnimation > durationOfDeath) durationOfAnimation = durationOfDeath;
        if (durationOfGameOver == 0f) durationOfGameOver = 10f;
        else if (durationOfGameOver < 0f) durationOfGameOver *= -1f;

    }

    void FixedUpdate()
    {
        
        // If the player is dead
        if (playerIsDead)
        {

            /* If the death timer has yet to reach zero, then it will tick down using unscaled time;
            otherwise, the reset or game over process will begin */
            if (durationOfDeath > 0f) durationOfDeath -= Time.fixedDeltaTime;
            else
            {

                if (!outOfLives)
                {

                    /* If the player has a positive number of lives remaining, then the respawn process will begin;
                    if not, then they will get a game over */
                    if (lcReference.playerLives > 0) ResetAnimation();
                    else InitiateGameOverSequence();

                }

            }

            /* If the zoom timer has yet to reach zero, then it will tick down using unscaled time;
            otherwise, it will no longer be allowed to zoom by way of the Boolean */
            if (durationOfAnimation > 0f) durationOfAnimation -= Time.fixedDeltaTime;
            else canZoomOut = false;

            // If the Boolean has the proper value, then the camera will zoom
            if (canZoomOut) CreateZoom();

        }

        // If the player is out of lives and in a game-over state
        if (outOfLives)
        {

            /* If the game over timer has yet to reach zero, then it will tick down using unscaled time;
            otherwise, the game over process will continue via another script */
            if (durationOfGameOver > 0f) durationOfGameOver -= Time.fixedDeltaTime;
            else
            {

                // If the statistics have not appeared yet;
                if (!cueStats)
                {

                    // Causes the text to disappear, including the UI text
                    tmpbReference.RemoteStart(tmpbReference.scoreText, false);
                    tmpbReference.RemoteStart(tmpbReference.levelText, false);
                    tmpbReference.RemoteStart(tmpbReference.timeText, false);
                    tmpbReference.RemoteStart(tmpbReference.lifeText, false);
                    tmpbReference.RemoteStart(tmpbReference.gameOverText, false);

                    // Provides the cue for them to appear
                    cueStats = true;

                }

            }

        }

    }

    public void PlayDeathSequence()
    {

        // Sets the appropriate Booleans
        playerIsDead = true;
        if (durationOfAnimation > 0f) canZoomOut = true;
        
        // Temporarily moves the player's camera to this object as a child
        playerCamera.transform.SetParent(gameObject.transform, false);

        // Gives the camera the appropriate initial offset
        playerCamera.transform.position = deathLocation + cameraOffset;

        // Points the camera at the specific spot of death
        playerCamera.transform.LookAt(deathLocation);

        // Creates the explosions
        Instantiate(playerExplosion, deathLocation, Quaternion.identity);
        Instantiate(explosionModule, deathLocation, Quaternion.identity);

    }

    private void CreateZoom()
    {

        // Moves the camera back at a given rate
        playerCamera.transform.Translate(zoomSpeed * Time.fixedDeltaTime * Vector3.back, Space.Self);

    }

    private void ResetAnimation()
    {

        // Moves the player back to the origin of the arena, nullifying any velocity still applied
        originalCameraParent.transform.SetPositionAndRotation(originalStartingCoordinates, Quaternion.identity);
        originalCameraParent.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        originalCameraParent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Returns the camera back to its original parent and nullifies any transformed position/rotation
        playerCamera.transform.SetParent(originalCameraParent.transform, false);
        playerCamera.transform.SetLocalPositionAndRotation(playerHeight, Quaternion.identity);

        // Reactivates the player
        originalCameraParent.SetActive(true);

        // Grants the player invincibility
        originalCameraParent.GetComponent<SpaceshipBehavior>().isInvincible = true;

        // Resets the public variables
        durationOfAnimation = originalAnimationLength;
        durationOfDeath = originalDeathLength;

        // Nullifies the dead status
        playerIsDead = false;

    }

    private void InitiateGameOverSequence()
    {

        // Allows the postmortem script to collect important player variables for display
        pmReference.CopyVariables();

        // Causes the text to appear
        tmpbReference.RemoteStart(tmpbReference.gameOverText, true);

        // Switches the kill Boolean to true to prevent excessive executions
        outOfLives = true;

    }

}
