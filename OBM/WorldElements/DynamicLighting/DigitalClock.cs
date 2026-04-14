/* Charlie Dye, PACE Team - 2024.12.03

This is the script for the clock in the starting room */

using System.Collections;
using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{

    [Header("Light to Rotate")]
    [Tooltip("The directional light, as in the Sun, in the scene.")] public GameObject directionalLight;

    [Header("Numerical Value")]
    [Tooltip("The time of day, converted from hours/minutes/seconds to only seconds. Updates automatically.")] public int timeInSeconds;

    [Header("Boolean Variable")]
    [Tooltip("Whether or not the time on the clock influences the position of the Sun (directional light).")] public bool checkForSunBehavior;

    // TextMeshPro variable
    private TextMeshPro textToBeDisplayed;

    // String variables
    private string dateAndTime = null;
    private readonly string[] monthArray = {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};

    void Start()
    {

        // Fetches the text
        textToBeDisplayed = gameObject.GetComponent<TextMeshPro>();

        // Starts the coroutine
        StartCoroutine(UpdateClock());
        
    }

    public IEnumerator UpdateClock()
    {

        while (textToBeDisplayed != null)
        {

            // Executes the method below
            ManipulateString();

            // Sets the text mesh as the string
            textToBeDisplayed.text = dateAndTime;

            // Waits one second before refreshing
            yield return new WaitForSecondsRealtime(1f);

        }

    }

    public void ManipulateString()
    {

        // Fetches real-time date and time information from the system, converting it into a string
        dateAndTime = System.DateTime.Now.ToString();

        // Divides that string into three, using the slash ("/") character as a separator
        string[] firstRoundSubstrings = dateAndTime.Split("/");

        // If the hour number is less than 10, then a zero will be appended to the front
        if (int.Parse(firstRoundSubstrings[1]) < 10) firstRoundSubstrings[1] = firstRoundSubstrings[1].Insert(0, "0");

        // Divides the third array element into a further three, using the colon (":") as a separator
        string[] secondRoundSubstrings = firstRoundSubstrings[2].Split(":");

        // Divides the first array element into another further two, using whitespace as a separator
        string[] thirdRoundSubstrings = secondRoundSubstrings[0].Split(" ");

        // If the 12-hour time is in the AM hours
        if (secondRoundSubstrings[2].Contains(" AM"))
        {

            // Removes the letters and preceding whitespace
            secondRoundSubstrings[2] = secondRoundSubstrings[2].Remove(secondRoundSubstrings[2].Length - 3, 3);

            // If the hour value is 12 (i.e., midnight), then the first character changes to a single zero digit
            if (int.Parse(thirdRoundSubstrings[1]) == 12) thirdRoundSubstrings[1] = "0";

            // If the hour value is less than 10, then a zero will be appended to the front of the value
            if (int.Parse(thirdRoundSubstrings[1]) < 10) thirdRoundSubstrings[1] = thirdRoundSubstrings[1].Insert(0, "0");

        }
        // If the 12-hour-time is in the PM hours
        else if (secondRoundSubstrings[2].Contains(" PM"))
        {

            // If the hour value is less than 12
            if (int.Parse(thirdRoundSubstrings[1]) < 12)
            {

                // Introduces a local int that adds 12 to the string value once converted to an int
                int militaryTime = int.Parse(thirdRoundSubstrings[1]) + 12;

                // Re-converts back into a string as 24-hour time
                thirdRoundSubstrings[1] = militaryTime.ToString();

            }

            // Removes the letters and preceding whitespace
            secondRoundSubstrings[2] = secondRoundSubstrings[2].Remove(secondRoundSubstrings[2].Length - 3, 3);

        }
        
        // Assembles everything into a workable string to be displayed
        dateAndTime = $"{monthArray[int.Parse(firstRoundSubstrings[0]) - 1]} {firstRoundSubstrings[1]}, {thirdRoundSubstrings[0]}" +
            $"\n{thirdRoundSubstrings[1]}:{secondRoundSubstrings[1]}:{secondRoundSubstrings[2]}";

        // If the bool is true, then the information gathered from the parsed string will translate into the Sun's rotation
        if (checkForSunBehavior && directionalLight != null) ConvertToRotation(int.Parse(thirdRoundSubstrings[1]),
            int.Parse(secondRoundSubstrings[1]), int.Parse(secondRoundSubstrings[2]));

    }

    public void ConvertToRotation(int hourValue, int minuteValue, int secondValue)
    {

        // Assembles the equation that calculates the sun's position using hour, minute, and second values
        timeInSeconds = (hourValue * 3600) + (minuteValue * 60) + secondValue;

        // Calculates the rotation of the sun based on real time
        directionalLight.transform.rotation = Quaternion.Euler(360 - (timeInSeconds / 240f) - 90f, 90f, 90f);

    }

}
