/* Charlie Dye - ECT 4440 - 2026.01.27

This is the script for asteroids splitting up when destroyed */

using Unity.Hierarchy;
using UnityEngine;

public class AsteroidMitosis : MonoBehaviour
{

    [Header("Asteroid Object")]
    [Tooltip("The size of the children asteroids that this one should spawn when destroyed.")] public GameObject childToSpawn;

    [Header("Numerical Variables")]
    [Tooltip("The maximum distance a child can spawn from the position transform of its parent when destroyed.")] public float maximumSpawnRadius;
    [Tooltip("The number of children to be spawned upon destruction of the parent.")] public int numberToSpawn;

    // Script reference
    private AsteroidBehavior abReference;

    void OnEnable()
    {
        
        // Fetches the script already attached to the asteroid
        abReference = gameObject.GetComponent<AsteroidBehavior>();

        // If the asteroid is the smallest possible size (i.e., it should not spawn children), then this script will self-destruct
        if (abReference.asteroidSize == 0) Destroy(this);

        // If the spawn radius is configured incorrectly, then this will fix it
        if (maximumSpawnRadius == 0) maximumSpawnRadius = 1f;
        else if (maximumSpawnRadius < 0f) maximumSpawnRadius *= -1f;

        // If the prefab variable is left null, then this script will self-destruct
        if (childToSpawn == null) Destroy(this);

        // If the number of children to spawn is configured incorrectly, then this will fix that
        if (numberToSpawn == 0) numberToSpawn = 2;
        else if (numberToSpawn < 0) numberToSpawn *= -1;

    }

    public void EjectFragments()
    {

        // Spawn a given number of children (the public prefab above) using a for-loop
        for (int i = 0; i < numberToSpawn; i++)
        {

            // Conditional statement that checks if the local integer is even or odd
            if (i % 2 == 0)
            {

                // Instantiates the children at the location where the parent was destroyed (with some randomness injected)
                Instantiate(childToSpawn, new Vector3(gameObject.transform.position.x + Random.Range(0f, maximumSpawnRadius),
                    0f, gameObject.transform.position.z + Random.Range(0f, maximumSpawnRadius)), Quaternion.identity);

            }
            else
            {

                // Instantiates the children at the location where the parent was destroyed (with some randomness injected)
                Instantiate(childToSpawn, new Vector3(gameObject.transform.position.x + Random.Range(-maximumSpawnRadius, 0f),
                    0f, gameObject.transform.position.z + Random.Range(-maximumSpawnRadius, 0f)), Quaternion.identity);

            }

        }


    }

}
