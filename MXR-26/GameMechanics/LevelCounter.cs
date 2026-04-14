/* Charlie Dye - ECT 4440 - 2026.01.26

This is the script for keeping track of the level and asteroid count */

using UnityEngine;

public class LevelCounter : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the player's UI.")] public TMPBehavior tmpReference;

    [Header("Asteroid Prefabs")]
    [Tooltip("The colossal (largest) size of asteroid. Stored as a prefab.")] public GameObject asteroidColossal;
    [Tooltip("The large (second largest) size of asteroid. Stored as a prefab.")] public GameObject asteroidLarge;
    [Tooltip("The medium (second smallest) size of asteroid. Stored as a prefab.")] public GameObject asteroidMedium;
    [Tooltip("The small (smallest) size of asteroid. Stored as a prefab.")] public GameObject asteroidSmall;

    [Header("Numerical Variables")]
    [Tooltip("The delay at the beginning.")] public float initialDelay;
    [Tooltip("The delay between \"levels\", or waves of asteroids.")] public float levelDelay;
    [Tooltip("The current level of the game.")] public int playerLevel;
    [Tooltip("The current score attained by the player.")] public int playerScore;
    [Tooltip("The starting amount of lives the player has.")] public int playerLives;
    [Tooltip("The amount of lives the player has lost during the current round.")] public int livesLostDuringRound;
    [Tooltip("The time elapsed during this round only.")] public float roundTimeElapsed;
    [Tooltip("The minimum distance that new asteroids can spawn from the arena center.")] public float minimumSpawnZone;
    [Tooltip("The current count of asteroids in the arena.")] public int asteroidsRemaining;
    [Tooltip("The amount of asteroids to begin the first level with.")] public int numberToSpawn;
    [Tooltip("The amount of bullets the player has fired so far this game.")] public int numberOfBulletsFired;
    [Tooltip("The amount of missiles the player has fired so far this game.")] public int numberOfMissilesFired;
    [Tooltip("The number of asteroids the player has destroyed so far this game.")] public int asteroidsShotDown;
    [Tooltip("The number of UFOs the player has destroyed so far this game.")] public int hostileShipsShotDown;
    [Tooltip("The amount of power-ups the player has collected so far during this game.")] public int powerUpsCollected;
    [Tooltip("An array of all scores that will give an extra life to the player when reached.")] public int[] extraLifeScores;
    [HideInInspector] public int lifeReward;
    [HideInInspector] public int timeReward;
    private int scoreIndexer = 0;

    [Header("Boolean Variable")]
    [Tooltip("Can a new wave of asteroids spawn?")] public bool canCreateNewAsteroids;
    private bool hasStarted = false;

    void Awake()
    {

        // Sets the numerical variables to proper values if they are not so already
        if (initialDelay == 0f) initialDelay = 3f;
        else if (initialDelay < 0f) initialDelay *= -1f;
        if (levelDelay == 0f) levelDelay = 1f;
        else if (levelDelay < 0f) levelDelay *= -1f;
        if (playerLevel != 1) playerLevel = 1;
        if (playerScore != 0) playerScore = 0;
        if (playerLives == 0) playerLives = 1;
        else if (playerLives < 0) playerLives *= -1;
        if (livesLostDuringRound != 0) livesLostDuringRound = 0;
        if (roundTimeElapsed != 0f) roundTimeElapsed = 0f;
        if (minimumSpawnZone == 0f) minimumSpawnZone = 100f;
        else if (minimumSpawnZone < 0f) minimumSpawnZone *= -1f;
        if (numberToSpawn == 0) numberToSpawn = 3;
        else if (numberToSpawn < 0) numberToSpawn *= -1;
        if (numberOfBulletsFired != 0) numberOfBulletsFired = 0;
        if (numberOfMissilesFired != 0) numberOfMissilesFired = 0;
        if (asteroidsShotDown != 0) asteroidsShotDown = 0;
        if (hostileShipsShotDown != 0) hostileShipsShotDown = 0;
        if (powerUpsCollected != 0) powerUpsCollected = 0;

        // Sets the asteroid spawn Boolean to true
        canCreateNewAsteroids = false;

    }

    void FixedUpdate()
    {

        // Counts down on the initial timer; when it reaches zero, it will create the first wave of asteroids
        if (!hasStarted)
        {

            if (initialDelay > 0f) initialDelay -= Time.fixedDeltaTime;
            else
            {

                DetermineWave(true);
                hasStarted = true;

            }

        }

        // Counts the number of asteroids (i.e., objects with the appropriate tag) and constantly updates the associated integer as such
        asteroidsRemaining = GameObject.FindGameObjectsWithTag("Asteroid").Length;

        // If the number of asteroids reaches zero (i.e., the level is cleared), a UFO is not present, and the Boolean value allows, then a new wave will spawn once the delay expires
        if (asteroidsRemaining == 0 && !GameObject.FindGameObjectWithTag("Hostile Spacecraft") && canCreateNewAsteroids)
        {

            // Activates the method after a delay
            Invoke(nameof(LevelTransition), levelDelay);

            // Sets the Boolean to false, preventing excessive executions
            canCreateNewAsteroids = false;

        }

        // If the round is active, then the round's elapsed time will begin ticking
        if (canCreateNewAsteroids) roundTimeElapsed += Time.fixedDeltaTime;

        // Checks if the player should earn an extra life if they have a high enough score
        CheckForExtraLives();

    }

    private void LevelTransition()
    {

        // Segues into the next level; this call, unlike the one in Start(), passes with a false argument
        DetermineWave(false);

    }

    private void DetermineWave(bool startingWave)
    {

        // If the wave to be generated is not the one to begin the experience
        if (!startingWave)
        {

            // Increments the level counter by one
            playerLevel++;

            // Mathematical conditional statements for determing the number of asteroids to spawn per wave, which incrementally rises as the levels advance
            if (playerLevel < 3) numberToSpawn++;
            else if (playerLevel >= 3 && playerLevel < 6) numberToSpawn += 2;
            else if (playerLevel >= 6 && playerLevel < 9) numberToSpawn += 3;
            else if (playerLevel >= 9 && playerLevel < 12) numberToSpawn += 4;
            else if (playerLevel >= 12) numberToSpawn += 5;

            // Gives the player a reward (or lack thereof) to their score, depending on their performance during the round
            EndOfRoundReward();

            // For-loop that repeats for each element in the warning TMP text array
            for (int i = 0; i < tmpReference.rewardTexts.Length; i++)
            {

                // Causes each line to type into view
                tmpReference.RemoteStart(tmpReference.rewardTexts[i], true);

            }

        }

        // Causes the round and appropriate number to appear
        tmpReference.RemoteStart(tmpReference.roundText, true);

        // Activates the method after a delay
        Invoke(nameof(CreateWave), levelDelay);

    }

    public void CreateWave()
    {

        // Causes the round and appropriate number to disappear
        tmpReference.RemoteStart(tmpReference.roundText, false);

        // For-loop that spawns the number of asteroids as stated by the spawn number integer
        for (int j = 0; j < numberToSpawn;  j++)
        {

            // Local random integers that determine the (positive/negative) value of each new asteroid's X- and Z- coordinates
            int randomLocationX = Random.Range(0, 2);
            int randomLocationZ = Random.Range(0, 2);

            // Local decimals whole values are positive or negative as a result of the above integers' random rolls
            float randomOutputX;
            float randomOutputZ;
            

            /* Builds the coordinates of the new asteroid with even more randomized values;
            the gap between positive and negative spawn zone exists to prevent asteroids from unfairly spawning too close to the origin (the player spawns there) */
            if (randomLocationX == 0) randomOutputX = Random.Range(minimumSpawnZone, 500f);
            else randomOutputX = Random.Range(-500f, -minimumSpawnZone);
            if (randomLocationZ == 0) randomOutputZ = Random.Range(minimumSpawnZone, 500f);
            else randomOutputZ = Random.Range(-500f, -minimumSpawnZone);

            // Vector-3 that stores the randomized spawn location using the randomized output floats
            Vector3 randomMasterOutput = new(randomOutputX, 0f, randomOutputZ);

            // Local random decimal that determines the size of asteroid to spawn per pass; this is weighted significantly in favor of spawning large asteroids
            float sizeDeterminer = Random.Range(0f, 100f);
            if (sizeDeterminer < 2.5f) Instantiate(asteroidSmall, randomMasterOutput, Quaternion.identity);
            else if (sizeDeterminer >= 2.5f && sizeDeterminer < 5f) Instantiate(asteroidMedium, randomMasterOutput, Quaternion.identity);
            else if (sizeDeterminer >= 5f && sizeDeterminer < 90f) Instantiate(asteroidLarge, randomMasterOutput, Quaternion.identity);
            else if (sizeDeterminer >= 90f) Instantiate(asteroidColossal, randomMasterOutput, Quaternion.identity);

        }

        // If the player is on level one, then this will not execute
        if (playerLevel > 1)
        {

            // For-loop that repeats for each element in the warning TMP text array
            for (int k = 0; k < tmpReference.rewardTexts.Length; k++)
            {

                // Causes each line to disappear from view
                tmpReference.RemoteStart(tmpReference.rewardTexts[k], false);

            }

        }

        // Sets the Boolean value back to true, allowing for subsequent executions once this newly-generated wave is destroyed
        canCreateNewAsteroids = true;

    }

    private void EndOfRoundReward()
    {

        /* Local integer that calculates the returned reward score for lack of lives lost;
        the if-statement is necessary to prevent a negative result */
        if (livesLostDuringRound <= 5) lifeReward = 5000 - (livesLostDuringRound * 1000);
        else lifeReward = 0;

        /* Local integerS that calculates the returned reward score for a shorter time to complete the round;
        the if-statement is necessary to prevent a negative result */
        int roundedTime = Mathf.FloorToInt(roundTimeElapsed);
        if (roundedTime <= 500) timeReward = 10000 - (roundedTime * 20);
        else timeReward = 0;

        // Appends the resulting reward (the sum of the two integers) to the player's score
        playerScore += lifeReward + timeReward;

        // Resets the round time and lost-life counters
        livesLostDuringRound = 0;
        roundTimeElapsed = 0f;

    }

    private void CheckForExtraLives()
    {

        // If the player's score is higher than the target (i.e., the value in the array given by the private indexer)
        if (playerScore >= extraLifeScores[scoreIndexer])
        {

            // Rewards an extra life
            playerLives++;

            // Increments the indexer by one, making the next target the next value defined in the array
            scoreIndexer++;

        }


    }

}
