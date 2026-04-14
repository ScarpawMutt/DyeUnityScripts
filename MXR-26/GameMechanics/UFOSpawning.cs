/* Charlie Dye - ECT 4440 - 2026.02.12

This is the script for spawning UFOs into the play area */

using System.Collections;
using UnityEngine;

public class UFOSpawning : MonoBehaviour
{

    [Header("Object Array")]
    [Tooltip("An array of all UFOs that have a chance to be spawned by this script.")] public GameObject[] hostileShipsToSelect;

    [Header("Numerical Variables")]
    [Tooltip("The current chance of a UFO spawning in the arena.")] public float currentSpawnChance;
    [Tooltip("The minimum chance, between zero and one hundred, required to spawn a UFO.")] public float minimumSpawnChance;
    [Tooltip("The increment in chance, expressed as a percentage, that a UFO has the chance to spawn per coroutine cycle.")] public float chanceIncrement;
    [Tooltip("The maximum distance from the X- and Z-axes that a UFO can spawn at as a constructed three-vector system.")] public float maximumSpawnDistance;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;
    [Tooltip("The minimum level the player must be at before UFOs can begin spawning.")] public int minimumLevel;
    private Vector3 masterRandomLocation;

    [Header("Boolean Variable")]
    [Tooltip("Is a UFO currently active in the arena?")] public bool shipIsInArena;
    private bool coroutineHasStarted = false;

    // Script references
    private LevelCounter lcReference;
    private UFOBehavior ufobReference;
    private UFOWeapons ufowReference;

    void Awake()
    {

        /* Searches this script's game object for the script reference; if that fails, then it searches the scene.
        If that still fails, then this script will self-destruct */
        if (gameObject.GetComponent<LevelCounter>()) lcReference = gameObject.GetComponent<LevelCounter>();
        else if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
        else Destroy(this);

        // If the power-up array is empty, then this script will self-destruct
        if (hostileShipsToSelect.Length == 0) Destroy(this);

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

        /* Scans the scene for objects with a UFO's controls script; if it returns a value indicating that one exists, then the appropriate Boolean will return true;
        otherwise, it will return false */
        if (FindFirstObjectByType<UFOBehavior>()) shipIsInArena = true;
        else shipIsInArena = false;

    }

    private IEnumerator MasterSpawner()
    {

        // While-loop that returns true as long as this script's object is not destroyed and the elements of the array are not wiped
        while (gameObject != null && hostileShipsToSelect.Length > 0)
        {

            // If a UFO is already in the arena, then no others can spawn until it is destroyed
            if (shipIsInArena)
            {

                // Keeps the spawn chance at zero
                currentSpawnChance = 0f;

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            // If a UFO is not in the arena, then one can spawn
            else
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
                        ConstructSpawnLocation();

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

    }

    private void ConstructSpawnLocation()
    {

        // Introduces local integers that help determine spawn location later (these behave like Booleans)
        int xComesBeforeZ;
        int spawnOnPositiveX;
        int spawnOnPositiveZ;

        // Introduces local float values that are a product of the randomization of the Booleans above
        float randomSpawnLocationX;
        float randomSpawnLocationZ;

        // Uses random values to determine the value of the integer
        xComesBeforeZ = Random.Range(0, 2);
        spawnOnPositiveX = Random.Range(0, 2);
        spawnOnPositiveZ = Random.Range(0, 2);

        /* Depending on the randomization of the integers, the exact spawn locations per axis is further randomized;
        this logic exists because UFOs can only spawn on an extremely specific positive/negative value relative to either the X- or Z- axis */
        if (xComesBeforeZ == 0)
        {

            // The X-position becomes the truly randomized variable, taking precedence over the Z-positon
            randomSpawnLocationX = Random.Range(-maximumSpawnDistance, maximumSpawnDistance);

            // Logic concerning the results of the respective integer value for the Z-position
            if (spawnOnPositiveZ == 0)
            {

                // Sets the Z-positon to a fixed value
                randomSpawnLocationZ = -maximumSpawnDistance;

                // Constructs the final location
                masterRandomLocation = new Vector3(randomSpawnLocationX, 0f, randomSpawnLocationZ);

            }
            else if (spawnOnPositiveZ == 1)
            {

                // Sets the Z-positon to a fixed value
                randomSpawnLocationZ = maximumSpawnDistance;

                // Constructs the final location
                masterRandomLocation = new Vector3(randomSpawnLocationX, 0f, randomSpawnLocationZ);

            }

        }
        else if (xComesBeforeZ == 1)
        {

            // The Z-position becomes the truly randomized variable, taking precedence over the X-positon
            randomSpawnLocationZ = Random.Range(-maximumSpawnDistance, maximumSpawnDistance);

            // Logic concerning the results of the respective integer value for the Z-position
            if (spawnOnPositiveX == 0)
            {

                // Sets the X-positon to a fixed value
                randomSpawnLocationX = -maximumSpawnDistance;

                // Constructs the final location
                masterRandomLocation = new Vector3(randomSpawnLocationX, 0f, randomSpawnLocationZ);

            }
            else if (spawnOnPositiveX == 1)
            {

                // Sets the X-positon to a fixed value
                randomSpawnLocationX = maximumSpawnDistance;

                // Constructs the final location
                masterRandomLocation = new Vector3(randomSpawnLocationX, 0f, randomSpawnLocationZ);

            }

        }

        // Local integer that acts as the indexer of the object array, assuming a random value per each method call
        int shipSelecter = Random.Range(0, hostileShipsToSelect.Length);

        // Local integers that determine the randomized behavior of the spawning UFO's movement and weapon algorithms
        int randomMovement = Random.Range(0, 2);
        int randomAggression = Random.Range(0, 3);

        ufobReference = hostileShipsToSelect[shipSelecter].GetComponent<UFOBehavior>();
        ufowReference = hostileShipsToSelect[shipSelecter].GetComponent<UFOWeapons>();

        if (randomMovement == 0) ufobReference.uemReference = UFOEnumMoving.Homing;
        else if (randomMovement == 1) ufobReference.uemReference = UFOEnumMoving.Roaming;

        if (randomAggression == 0) ufowReference.uefReference = UFOEnumFiring.Fighter;
        else if (randomAggression == 1) ufowReference.uefReference = UFOEnumFiring.Minesweeper;
        else if (randomAggression == 2) ufowReference.uefReference = UFOEnumFiring.Gunship;

        // Spawns the UFO selected with a randomized location
        Instantiate(hostileShipsToSelect[shipSelecter], masterRandomLocation, Quaternion.identity);

    }

}
