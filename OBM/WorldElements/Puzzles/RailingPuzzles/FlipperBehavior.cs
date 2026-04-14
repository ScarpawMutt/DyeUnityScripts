/* Charlie Dye, PACE Team - 2025.12.04

This is the script for the flippers on the railing puzzle gadgets */

using System.Collections;
using UnityEngine;

public class FlipperBehavior : MonoBehaviour
{

    [Header("Flipper Object")]
    [Tooltip("The flipper object to be animated.")] public GameObject flipperObject;

    [Header("Float Variables")]
    [Tooltip("The maximum angle at which the flipper can point away from neutral position, either up or down.")] public float maximumFlipperAngle;
    [Tooltip("The shortest possible time that the flipper can remain in a given position.")] public float minimumSwitchTime;
    [Tooltip("The longest possible time that the flipper can remain in a given position.")] public float maximumSwitchTime;
    private float randomSwitchTime;
    private float originalAngle;

    [Header("Boolean Variables")]
    [Tooltip("Is the flipper supposed to point up (true) or down (false)?")] public bool switchPointsUp;
    [Tooltip("Can the flipper animate?")] public bool canSwitch;
    private bool hasStarted;

    void Start()
    {

        // If the flipper object is left null, then this script will self-destruct
        if (flipperObject == null) Destroy(this);

        // If the float variables are configured incorrectly, then this will correct them
        if (maximumFlipperAngle == 0f) maximumFlipperAngle = 10f;
        else if (maximumFlipperAngle < 0f) maximumFlipperAngle *= -1f;
        if (minimumSwitchTime == 0f) minimumSwitchTime = 0.5f;
        else if (minimumSwitchTime < 0f) minimumSwitchTime *= -1f;
        if (maximumSwitchTime == 0f) maximumSwitchTime = 1f;
        else if (maximumSwitchTime < 0f) maximumSwitchTime *= -1f;
        if (maximumSwitchTime <= minimumSwitchTime) maximumSwitchTime = minimumSwitchTime + 1f;

        // If the Boolean variables are configured incorrectly, then this will correct them
        if (canSwitch) canSwitch = false;
        if (hasStarted) hasStarted = false;

        // Logs the original angle for future use
        originalAngle = flipperObject.transform.localRotation.eulerAngles.x;

        // Depending on the Boolean value, the flipper moves to the appropriate position
        if (switchPointsUp) flipperObject.transform.localRotation = Quaternion.Euler(originalAngle - maximumFlipperAngle, 0f, 0f);
        else flipperObject.transform.localRotation = Quaternion.Euler(originalAngle + maximumFlipperAngle, 0f, 0f);

    }

    void FixedUpdate()
    {
        
        // If the flipper is not switching yet but must begin
        if (canSwitch && !hasStarted)
        {

            // Executes the coroutine with the Boolean variable
            StartCoroutine(RandomizeSwitching());

            // Kill bool for only one execution
            hasStarted = true;

        }

    }

    public void RotateFlipper()
    {

        // If the flipper must point up
        if (switchPointsUp) flipperObject.transform.localRotation = Quaternion.Euler(originalAngle - maximumFlipperAngle, 0f, 0f);
        // If the flipper must point down
        else flipperObject.transform.localRotation = Quaternion.Euler(originalAngle + maximumFlipperAngle, 0f, 0f);

    }

    public IEnumerator RandomizeSwitching()
    {

        // While-loop ensuring that the flipper object is not null
        while (flipperObject != null)
        {

            // If the flipper can switch
            if (canSwitch)
            {

                // Randomizes the coroutine refresh rate using the entered minimum and maximum values
                randomSwitchTime = Random.Range(minimumSwitchTime, maximumSwitchTime);

                // Inverts the current Boolean value
                if (!switchPointsUp) switchPointsUp = true;
                else switchPointsUp = false;

                // Calls the flip method
                RotateFlipper();

                // Refreshes the coroutine using randomized time
                yield return new WaitForSecondsRealtime(randomSwitchTime);

            }
            // Breaks the coroutine
            else yield break;

        }

    }

}
