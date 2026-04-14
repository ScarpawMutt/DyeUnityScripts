/* Charlie Dye - ECT 4440 - 2026.01.29

This is the script for the text in the UI */

using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TMPBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the death mechanic.")] public DeathSequence dsReference;
    [Tooltip("The script attached to the level counter.")] public LevelCounter lcReference;

    [Header("Float References")]
    [Tooltip("The delay before the updated values first appear in the UI. This can be left at zero to skip the coroutine.")] public float updateDelay;
    [Tooltip("The refresh rate of the coroutine. Also works as the delay between each letter of a string appearing.")] public float refreshRate;
    private float timeElapsed = 0f;

    [Header("Text UI References")]
    [Tooltip("The text that keeps track of the score.")] public TextMeshProUGUI scoreText;
    [Tooltip("The text that keeps track of the current level.")] public TextMeshProUGUI levelText;
    [Tooltip("The text that keeps track of the time elapsed since the beginning.")] public TextMeshProUGUI timeText;
    [Tooltip("The text that keeps track of the amount of lives the player has.")] public TextMeshProUGUI lifeText;
    [Tooltip("The game over text.")] public TextMeshProUGUI gameOverText;
    [Tooltip("The counter that appears if the player is out of bounds.")] public TextMeshProUGUI countdownText;
    [Tooltip("The large text that appears once a new round begins.")] public TextMeshProUGUI roundText;
    [Tooltip("The large text that appears to congratulate the player post-game.")] public TextMeshProUGUI congratulationsText;
    [Tooltip("The array of text that warns the player when they stray too far from the arena.")] public TextMeshProUGUI[] warningTexts;
    [Tooltip("The array of text that appears whenever the player receives their end-of-round reward.")] public TextMeshProUGUI[] rewardTexts;
    [Tooltip("The array of text that displays the player's postmortem statistics once a \"game over\" is attained.")] public TextMeshProUGUI[] postmortemTexts;
    [Tooltip("The character that temporarily appears while the string is animated. Only a single character is ideal.")] public string spacerCharacter;

    // Boolean variable
    private bool canUpdate = false;

    void Awake()
    {

        // If the script reference is left null, then this script will self-destruct
        if (lcReference == null) Destroy(this);

        // Sets certain text boxes as inactive
        gameOverText.gameObject.SetActive(false);
        roundText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        congratulationsText.gameObject.SetActive(false);

        for (int i = 0; i < warningTexts.Length; i++)
        {

            warningTexts[i].gameObject.SetActive(false);

        }

        for (int j = 0; j < rewardTexts.Length; j++)
        {

            rewardTexts[j].gameObject.SetActive(false);

        }

        for (int k = 0; k < postmortemTexts.Length; k++)
        {

            postmortemTexts[k].gameObject.SetActive(false);

        }

    }

    void Start()
    {

        // If the floats have improper values, then this will correct them
        if (refreshRate == 0f) refreshRate = 0.03f;
        else if (refreshRate < 0f) refreshRate *= -1f;
        if (updateDelay < 0f) updateDelay = 0f;

        // If the spacer string has a null value, then this will correct it
        if (spacerCharacter.Length == 0) spacerCharacter = "_";

        // Initiates the startup sequence in the camera UI
        if (updateDelay > 0f) StartCoroutine(ProceduralAppearance(scoreText, true, false));
        if (updateDelay > 0f) StartCoroutine(ProceduralAppearance(levelText, true, false));
        if (updateDelay > 0f) StartCoroutine(ProceduralAppearance(timeText, true, false));
        if (updateDelay > 0f) StartCoroutine(ProceduralAppearance(lifeText, true, false));

    }

    void Update()
    {

        // Constantly updates the player's score, life, and level through the script reference and the method, if allowed and the game has not ended
        if (canUpdate && !dsReference.outOfLives) GetPlayerValues();

    }

    void FixedUpdate()
    {

        // If there is a delay, then it will count down; otherwise, it will allow for values in the UI to update
        if (updateDelay > 0f && !canUpdate) updateDelay -= Time.fixedDeltaTime;
        else canUpdate = true;

        // Counts the time, recording it on the private float, as long as the player has at least one life
        if (lcReference.playerLives > 0) timeElapsed += Time.fixedDeltaTime;

        // Manipulates the raw time into a more organized format, if allowed and the game has not ended
        if (canUpdate && !dsReference.outOfLives) CountTime();

    }

    private void GetPlayerValues()
    {

        // Refreshes the current level and score as the values recorded on the level counter script
        scoreText.text = $"SCORE: {lcReference.playerScore}";
        levelText.text = $"LEVEL: {lcReference.playerLevel}";
        lifeText.text = $"LIVES: {lcReference.playerLives}";

    }

    private void CountTime()
    {

        // Introduces local second, minute, and hour integers, and performs the calculations to get them into appropriate values
        int secondsElapsed = Mathf.FloorToInt(timeElapsed % 60f);
        int minutesElapsed = Mathf.FloorToInt(timeElapsed % 3600f / 60f);
        int hoursElapsed = Mathf.FloorToInt(timeElapsed / 3600f);

        // Updates the time elapsed using the hour-minute-second format as constructed by the above values
        timeText.text = "TIME ELAPSED: " + string.Format("{0:00}:{1:00}:{2:00}", hoursElapsed, minutesElapsed, secondsElapsed);

    }

    public void RemoteStart(TextMeshProUGUI textArgument, bool boolArgument)
    {

        // If the text's game object is still inactive, then this will activate it
        if (!textArgument.gameObject.activeInHierarchy) textArgument.gameObject.SetActive(true);

        // This method serves as a way for other scripts to start the following coroutine
        StartCoroutine(ProceduralAppearance(textArgument, boolArgument, true));

    }

    private IEnumerator ProceduralAppearance(TextMeshProUGUI textToAnimate, bool fadeIn, bool overrideDelay)
    {

        // If a certain text is being passed as the argument, then it will update with the appropriate value appended to the end
        if (textToAnimate == roundText) roundText.text = String.Concat(textToAnimate.text, lcReference.playerLevel);
        else if (textToAnimate == rewardTexts[1]) rewardTexts[1].text = String.Concat(textToAnimate.text, lcReference.lifeReward);
        else if (textToAnimate == rewardTexts[3]) rewardTexts[3].text = String.Concat(textToAnimate.text, lcReference.timeReward);

        // If the text to display is colored anything else other than white, then this will set it to display white for better visibility
        if (textToAnimate.color != Color.white) textToAnimate.color = Color.white;

        // Introduces a local integer that acts as the indexer
        int textIndexer;

        // Introduces a string that copies the original TMP's contents
        string duplicateText = textToAnimate.text;

        // If the text needs to appear letter by letter
        if (fadeIn)
        {

            // Introduces a null "animated" string
            string animatedText = null;

            // Sets the index value to zero
            textIndexer = 0;

            // While-loop that returns true as long as the duplicate text is not wiped and the original text's object is not null
            while (duplicateText != null && textToAnimate.gameObject != null)
            {

                // If the indexer is less than or equal to the TMP string's length
                if (textIndexer <= duplicateText.Length)
                {

                    // Concatenates the text into a single string containing the animated text and the spacer, always at the very end
                    textToAnimate.text = String.Concat(animatedText, spacerCharacter);

                    // Appends each character of the copy text onto the animated one, one at a time, if the indexer will not go out of bounds
                    if (textIndexer <= duplicateText.Length - 1) animatedText += duplicateText[textIndexer];

                    // Increases the indexer
                    textIndexer++;

                    /* If the value update has not occurred yet, or if it has but the coroutine has permission to override it, the coroutine will refresh;
                    otherwise, it will break */
                    if (!canUpdate || (canUpdate && overrideDelay)) yield return new WaitForSecondsRealtime(refreshRate);
                    else yield break;

                }
                // If there are no more characters to copy
                else
                {

                    // Removes the spacer off of the string by taking its length and subtracting appropriately
                    textToAnimate.text = textToAnimate.text.Remove(textToAnimate.text.Length - spacerCharacter.Length);

                    // Breaks the coroutine
                    yield break;

                }

            }

        }
        // If the text needs to disappear letter by letter
        else
        {

            // Sets the indexer to the length of the text minus one (to account for the value of zero)
            textIndexer = textToAnimate.text.Length - 1;

            // While-loop that returns true as long as the text's object is not destroyed
            while (textToAnimate.gameObject != null)
            {

                // If the indexer is greater than or equal to zero
                if (textIndexer >= 0)
                {

                    // Removes the final character from the current string
                    textToAnimate.text = textToAnimate.text.Remove(textIndexer);

                    // Decrements the indexer by one
                    textIndexer--;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the indexer is zero
                else
                {

                    // Resets the text back to its copied value
                    textToAnimate.text = duplicateText;

                    // If the round text is the text argument passed
                    if (textToAnimate == roundText)
                    {

                        /* Parses the current string, using the space as the dividing character;
                        it then whittles the string down to only the first element of the array */
                        string[] parsedRoundString = roundText.text.Split(" ");
                        roundText.text = parsedRoundString[0] + " ";

                    }
                    // If the reward texts are the text arguments passed
                    else if (textToAnimate == rewardTexts[1])
                    {

                        /* Parses the current string, using the space as the dividing character;
                        it then whittles the string down to only the first element of the array */
                        string[] parsedLifeString = rewardTexts[1].text.Split(" ");
                        rewardTexts[1].text = parsedLifeString[0] + " ";

                    }
                    else if (textToAnimate == rewardTexts[3])
                    {

                        /* Parses the current string, using the space as the dividing character;
                        it then whittles the string down to only the first element of the array */
                        string[] parsedTimeString = rewardTexts[3].text.Split(" ");
                        rewardTexts[3].text = parsedTimeString[0] + " ";

                    }
                    // If the countdown timer is the text argument passed
                    else if (textToAnimate == countdownText) countdownText.text = null;

                    // Deactivates the text box
                    textToAnimate.gameObject.SetActive(false);

                    // Breaks the coroutine
                    yield break;

                }

            }

        }

    }

}