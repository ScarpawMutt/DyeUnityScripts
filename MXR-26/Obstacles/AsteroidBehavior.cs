/* Charlie Dye - ECT 4440 - 2026.01.26

This is the script for asteroid behavior */

using UnityEngine;

public class AsteroidBehavior : MonoBehaviour
{

    [Header("Float Variables")]
    [Tooltip("The maximum rotational speed of the asteroid.")] public float maximumRotation;
    [Tooltip("The maximum allowable base speed of the asteroid.")] public float asteroidVelocity;
    [Tooltip("The maximum distance the asteroid can stray from the center of the arena before it must turn back. This value should always be at least 500.")] public float hardBound;
    [Tooltip("The size of the asteroid. This value denotes how many generations of children it will create when destroyed.")] public int asteroidSize;
    [Tooltip("The amount of points that this size of asteroid will reward when destroyed.")] public int pointsToAdd;
    private float actualVelocity;
    private float directionOutputX;
    private float directionOutputZ;
    private float massMultiplier;

    [Header("Audio")]
    [Tooltip("The \"blip\" that plays on the player's radar when it detects this object.")] public AudioSource radarBlip;

    void OnEnable()
    {

        // Calculates the output speed of the asteroid, depending on its size
        if (asteroidSize == 0) actualVelocity = asteroidVelocity;
        else if (asteroidSize == 1) actualVelocity = asteroidVelocity * 1.25f;
        else if (asteroidSize == 2) actualVelocity = asteroidVelocity * 1.5f;
        else if (asteroidSize == 3) actualVelocity = asteroidVelocity * 2f;

        // If the asteroid's bound setting is configured improperly, then this will fix it
        if (hardBound < 0f) hardBound *= -1f;
        if (hardBound < 500f) hardBound = 500f;

        // If the asteroid awards no points, then it will be reset by default
        if (pointsToAdd == 0) pointsToAdd = 10;
        else if (pointsToAdd < 0) pointsToAdd *= -1;

        /* The mass multiplier is altered depending on the asteroid's size;
        it is expressed as a function where the asteroid size serves as an exponent to the base of fifty */
        massMultiplier = 50 ^ asteroidSize;

        // Creates rotation along all three of the asteroid's axes by adding or subtracting to/from its angular velocity vectors
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-maximumRotation, maximumRotation), Random.Range(-maximumRotation, maximumRotation), Random.Range(-maximumRotation, maximumRotation));

        // Randomizes the child's initial rotation (this is merely a visual effect)
        gameObject.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        // Builds a random linear velocty for the asteroid to follow
        RandomizeDirection();

        // Moves the asteroid laterally
        MoveAsteroid();

    }

    private void RandomizeDirection()
    {

        // Local integers to be randomized
        int randomDirectionX = Random.Range(0, 2);
        int randomDirectionZ = Random.Range(0, 2);

        /* Constructs a randomized initial direction as a result of the earlier random integers;
        the gap closer to zero exists to prevent stationary / too-slow-moving asteroids */
        if (randomDirectionX == 0) directionOutputX = Random.Range(actualVelocity / 2f, actualVelocity);
        else directionOutputX = Random.Range(-actualVelocity, -actualVelocity / 2f);
        if (randomDirectionZ == 0) directionOutputZ = Random.Range(actualVelocity / 2f, actualVelocity);
        else directionOutputZ = Random.Range(-actualVelocity, -actualVelocity / 2f);

    }

    private void MoveAsteroid()
    {

        // Causes the asteroid to move in a random direction by adding or subtracting to/from its linear velocity vectors
        gameObject.GetComponent<Rigidbody>().AddForce(directionOutputX * massMultiplier, 0f, directionOutputZ * massMultiplier);

    }

    public void CreateRadarBlip()
    {

        // If the ship's radar system hits this object (i.e., detects it with a trigger collider), then this object will play a spatialized blip
        radarBlip.Play();

    }

}
