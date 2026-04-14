/* Charlie Dye - ECT 4440 - 2026.02.07

This is the script for spawning power-ups into the play area */

using System.Collections;
using UnityEngine;

public class PowerUpSpawning : MonoBehaviour
{

    [Header("Power-Up Array")]
    [Tooltip("An array of all power-ups that have a chance to be spawned by this script.")] public GameObject[] powerUpsToSelect;

    [Header("Numerical Variables")]
    [Tooltip("The current chance of a power-up spawning in the arena.")] public float currentSpawnChance;
    [Tooltip("The minimum chance, between zero and one hundred, required to spawn a power-up.")] public float minimumSpawnChance;
    [Tooltip("The increment in chance, expressed as a percentage, that a power-up has the chance to spawn per coroutine cycle.")] public float chanceIncrement;
    [Tooltip("The maximum distance from the arena's center that a power-up can spawn at, expressed in both the X- and Z-axes.")] public float maximumSpawnDistance;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;
    [Tooltip("The minimum level the player must be at before power-ups can begin spawning.")] public int minimumLevel;

    // Script reference
    private LevelCounter lcReference;

    // Boolean variable
    private bool coroutineHasStarted = false;

    void Awake()
    {

        /* Searches this script's game object for the script reference; if that fails, then it searches the scene.
        If that still fails, then this script will self-destruct */
        if (gameObject.GetComponent<LevelCounter>()) lcReference = gameObject.GetComponent<LevelCounter>();
        else if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
        else Destroy(this);

        // If the power-up array is empty, then this script will self-destruct
        if (powerUpsToSelect.Length == 0) Destroy(this);

    }

    void Start()
    {

        // If the numerical variables have improper values, then this will correct them
        if (minimumSpawnChance > 100f) minimumSpawnChance = 10f;
        else if (minimumSpawnChance < 0f) minimumSpawnChance *= -1f;
        if (chanceIncrement == 0f || chanceIncrement >= 100f) chanceIncrement = 0.1f;
        else if (chanceIncrement < 0f) chanceIncrement *= -1f;
        if (maximumSpawnDistance == 0f) maximumSpawnDistance = 400f;
        else if (maximumSpawnDistance < 1f) maximumSpawnDistance *= -1f;
        if (refreshRate == 0f) refreshRate = 1f;
        else if (refreshRate < 1f) refreshRate *= -1f;
        if (minimumLevel == 0) minimumLevel = 1;
        else if (minimumLevel < 1) minimumLevel *= -1;

        // Sets the initial spawning chance to zero
        currentSpawnChance = 0f;

    }

    void FixedUpdate()
    {
        
        // If the player has reached the minimum level required for the spawning process, then it will proceed accordingly
        if (minimumLevel <= lcReference.playerLevel && !coroutineHasStarted)
        {

            // Starts the coroutine
            StartCoroutine(MasterSpawner());

            // Changes the Boolean's value to prevent multiple executions
            coroutineHasStarted = true;

        }

    }

    private IEnumerator MasterSpawner()
    {

        // While-loop that returns true as long as this script's object is not destroyed and the elements of the array are not wiped
        while (gameObject != null && powerUpsToSelect.Length > 0)
        {

            // If the level is active (i.e., the player is not currently transitioning between levels)
            if (lcReference.canCreateNewAsteroids)
            {

                // Local decimal that assumes a random value between the minimum chance and 100 each coroutine cycle
                float minimumSpawnRequirement = Random.Range(minimumSpawnChance, 100f);

                // If the current chance of spawning is greater than or equal to the random decimal
                if (currentSpawnChance >= minimumSpawnRequirement)
                {

                    // A successful spawn occurs; this activates the method
                    SelectPowerUp();

                    // Resets the chance of spawning back to zero
                    currentSpawnChance = 0f;

                }
                // Otherwise, the spawn attempt fails; the chance of a successful spawn continues to increase
                else currentSpawnChance += chanceIncrement;

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            // Otherwise, the coroutine will refresh until the new level begins
            else yield return new WaitForSecondsRealtime(refreshRate);

        }

    }

    private void SelectPowerUp()
    {

        // Local integer that acts as the indexer of the object array, assuming a random value per each method call
        int powerUpSelecter = Random.Range(0, powerUpsToSelect.Length);

        // Spawns the power-up selected with a randomized location
        Instantiate(powerUpsToSelect[powerUpSelecter], new Vector3(Random.Range(-maximumSpawnDistance, maximumSpawnDistance), 0f,
            Random.Range(-maximumSpawnDistance, maximumSpawnDistance)), Quaternion.identity);

    }

}
