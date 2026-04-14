/* Charlie Dye - ECT 4440 - 2026.04.03

This is the script that awakens critical scripts in a scene once the transition has completed */

using UnityEngine;

public class AutomaticStart : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script that controls core level mechanics.")] public LevelCounter lcReference;
    [Tooltip("The script that controls the ship's shield.")] public ShieldBehavior shieldbReference;
    [Tooltip("The script that controls player movement.")] public SpaceshipBehavior spacebReference;
    [Tooltip("The script that manages text in the player's UI.")] public TMPBehavior tmpbReference;
    [Tooltip("The script that controls the transition animation between scenes.")] public TransitionScreen tsReference;
    [Tooltip("The script that handles the player's weapon systems.")] public WeaponBehavior wbReference;

    [Header("Boolean Variable")]
    [Tooltip("Is this script instance loaded in the main (gameplay) scene?")] public bool isInMainScene;
    private bool transitionHasCompleted = false;

    void Awake()
    {

        /* If the transition script is left null, then this script will attempt to locate it;
        if that fails, then this script will self-destruct, regardless of whether this is the main scene or not */
        if (tsReference == null)
        {

            if (FindFirstObjectByType<TransitionScreen>()) tsReference = FindFirstObjectByType<TransitionScreen>();
            else Destroy(this);

        }

        // If the transition effect has bad variable values, then this will correct them
        if (tsReference.isFadedOut) tsReference.isFadedOut = false;
        if (tsReference.attachedImage.color.a < 1f) tsReference.attachedImage.color = new(tsReference.attachedImage.color.r, tsReference.attachedImage.color.g,
            tsReference.attachedImage.color.b, 1f);

        // If this script is located in the main scene
        if (isInMainScene)
        {

            /* If any of the script references are left null, then this script will attempt to locate them;
            if that fails for any of them, then this script will self-destruct.
            The Boolean value is important because the scripts may be left null in non-main scenes without consequence
            (chances are likely that no such script instances exist in those scenes, as they are not needed) */
            if (lcReference ==  null)
            {

                if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
                else Destroy(this);

            }
            if (shieldbReference == null)
            {

                if (FindFirstObjectByType<ShieldBehavior>()) shieldbReference = FindFirstObjectByType<ShieldBehavior>();
                else Destroy(this);

            }
            if (spacebReference == null)
            {
                
                if (FindFirstObjectByType<SpaceshipBehavior>()) spacebReference = FindFirstObjectByType<SpaceshipBehavior>();
                else Destroy(this);

            }
            if (tmpbReference == null)
            {

                if (FindFirstObjectByType<TMPBehavior>()) tmpbReference = FindFirstObjectByType<TMPBehavior>();
                else Destroy(this);

            }
            if (wbReference == null)
            {

                if (FindFirstObjectByType<WeaponBehavior>()) wbReference = FindFirstObjectByType<WeaponBehavior>();
                else Destroy(this);

            }

            // If any of the non-transition script references are currently active, then this will disable them for now
            if (lcReference.enabled) lcReference.enabled = false;
            if (shieldbReference.enabled)shieldbReference.enabled = false;
            if (spacebReference.enabled) spacebReference.enabled = false;
            if (tmpbReference.enabled) tmpbReference.enabled = false;
            if (wbReference.enabled) wbReference.enabled = false;

        }

    }

    void Start()
    {

        // Starts the coroutine, allowing for the transition process to conclude
        tsReference.InitiateTransition(false);

    }

    void FixedUpdate()
    {

        // If this script is located in the main scene
        if (isInMainScene)
        {

            // If the transition has completed, but this script has not yet registered it as so
            if (tsReference.isFadedOut && !transitionHasCompleted)
            {

                // Activates the inactive scripts, allowing for proper gameplay
                lcReference.enabled = true;
                shieldbReference.enabled = true;
                spacebReference.enabled = true;
                tmpbReference.enabled = true;
                wbReference.enabled = true;

                // Switches the kill Boolean to true
                transitionHasCompleted = true;

            }

        }

    }

}
