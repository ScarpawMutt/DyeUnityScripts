/* Charlie Dye - ECT 4440 - 2026.03.17

This is the script for explosion behavior and management */

using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{

    [Header("Float Variable")]
    [Tooltip("The duration that this explosion remains in the scene as an object regardless of the visual cues presented.")] public float explosionTimer;

    void OnEnable()
    {

        // If the timer has an unworkable value, then this will correct it
        if (explosionTimer == 0f) explosionTimer = 1f;
        else if (explosionTimer < 0f) explosionTimer *= -1f;

    }

    void FixedUpdate()
    {

        /* If the timer is greater than zero, then it will subtract unscaled time;
        otherwise, the object (particle system prefab) this script is attached to will be destroyed */
        if (explosionTimer > 0f) explosionTimer -= Time.fixedDeltaTime;
        else Destroy(gameObject);

    }

}
