/* Charlie Dye, PACE Team - 2024.01.18

This is the script for the fire spawning */

using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireSpawner : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the fifth floor nut puzzle.")] public NutBehavior nbReference;
    private FireBehavior fb_reference;

    [Header("Objects")]
    [Tooltip("The fire object to be spawned.")] public GameObject firePrefab;
    [Tooltip("The dialogue text box for the lugnut gimmick.")] public GameObject nutTextBox;
    private GameObject fire_spawned;

    [Header("Extinguisher Spray Prefab")]
    [Tooltip("The spray collider attached to the fire extinguisher object.")] public GameObject extinguisherSpray;

    [Header("Possible Fire Spawn Transforms")]
    [Tooltip("The array containing all possible locations for fire to spawn.")] public Transform[] spawnLocations;

    [Header("Spawn Attributes")]
    [Tooltip("The rate, in seconds, at which fire spreads.")] public float spawnRate;

    [Header("Clamped Fire Parameters")]
    [Tooltip("The maximum allowed number of fire objects.")] [Range(1, 12)] public int fireLimit;
    [Tooltip("The number of times that the spawning coroutine has successfully executed.")] public int numberOfTimesCalled;

    [Header("Boolean Variables")]
    [Tooltip("Whether or not the maximum allowed number of fires have spawned.")] public bool fireAtCapacity = false;
    [Tooltip("Whether or not the spawner can begin spawning fire.")] public bool canBeginSpawning = false;
    [Tooltip("Whether or not fire has already spawned. Kill bool.")] public bool fireHasExecuted = false;
    [Tooltip("Whether or not all fires have been successfully extinguished. Kill bool.")] public bool fireHasBeenPutOut = false;
    [Tooltip("The array of booleans whose values correspond to occupied and unoccupied transforms in the earlier transform array.")] public bool[] transformOccupations;

    void Start()
    {

        // Creates elements in the bool array to match the length of the transform array
        transformOccupations = new bool[spawnLocations.Length];

        // Sets the initial coroutine call number to zero, if it is not already
        if (numberOfTimesCalled != 0) numberOfTimesCalled = 0;

        // Disables the nut text box for now, if not already so
        if (nutTextBox.activeInHierarchy) nutTextBox.SetActive(false);

    }

    void FixedUpdate()
    {

        // If the spawner can begin spawning, it will do so once the master boolean returns true via the SecondFloorRail.cs
        if (canBeginSpawning && !fireHasExecuted)
        {

            // Starts the coroutine with a single kill Boolean execution
            StartCoroutine(SpawnFire());
            fireHasExecuted = true;

        }

        // If at least one fire has been spawned but no more are left, then the lugnut will allow rotation
        if (numberOfTimesCalled >= 1 && GameObject.FindGameObjectsWithTag("Fire").Length == 0 && !fireHasBeenPutOut)
        {

            // Switches necessary Boolean values
            nbReference.canScrewIn = true;
            fireHasBeenPutOut = true;

            // Enables the text box for future dialogue
            nutTextBox.SetActive(true);

        }

        /* Counts the number of times the coroutine has completed successfully;
        as long as that number is less than the fire limit, the fire will continue to clone */
        if (numberOfTimesCalled >= fireLimit) fireAtCapacity = true;
        else fireAtCapacity = false;

    }

    public void SelectRandomLocation()
    {

        // Introduces a local integer that is randomized each time the method is called
        int randomArrayIndexer = Random.Range(0, spawnLocations.Length - 1);

        // If the transform is not already occupied by fire, according to the bool array
        if (!transformOccupations[randomArrayIndexer])
        {

            // Instantiates a new fire gameObject in the empty slot
            fire_spawned = Instantiate(firePrefab, spawnLocations[randomArrayIndexer]);

            // Accesses that new fire's Firebehavior.cs script and assigns the extinguisher's "local" spray collider as its own 
            fb_reference = fire_spawned.GetComponent<FireBehavior>();
            if (fb_reference.localSprayVariable == null) fb_reference.localSprayVariable = extinguisherSpray;

            // Marks the corresponding bool as true to prevent duplicated fires occupying the same space
            transformOccupations[randomArrayIndexer] = true;

            // Increments the number of successful coroutine calls by one
            numberOfTimesCalled++;

        }
        

    }

    public IEnumerator SpawnFire()
    {

        // If the maximum number of fires have not been reached
        while (!fireAtCapacity)
        {

            // Selects a location for fire to spawn
            SelectRandomLocation();

            // The coroutine repeats itself after an interval of given time, in seconds
            yield return new WaitForSecondsRealtime(spawnRate);

        }

    }

}