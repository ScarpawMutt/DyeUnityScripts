/* Charlie Dye, PACE Team - 2025.10.06

This is the script for the flag animation in the background */

using System.Collections;
using UnityEngine;

public class FlagBehavior : MonoBehaviour
{

    [Header("Flag GameObject")]
    [Tooltip("The flag to apply this behavior to.")] public GameObject flagObject;

    [Header("Float Variables")]
    [Tooltip("The speed that the flag shifts.")] public float windSpeed;
    [Tooltip("The angle, in degrees, that the flag fabric can shift from neutral position.")] public float windIntensity;
    [Tooltip("The frequncy that the wind changes direction.")] public float windVariability;
    [Tooltip("A two-value array that stores minimum and maximum values for the wind variability.")] public float[] minimaAndMaxima;
    [Tooltip("A two-value array that stores minimum and maximum values for the wind speed.")] public float[] gustsAndGales;
    private float originalFlagRotation;

    [Header("Boolean Variable")]
    [Tooltip("Is the flag rotating clockwise?")] public bool rotateClockwise = false;

    void Start()
    {

        // If the flag object is left null, then this script instance will self-destruct
        if (flagObject == null) Destroy(this);

        // Automatic correction controls for the float variables
        if (minimaAndMaxima.Length != 2) minimaAndMaxima = new float[2];
        if (minimaAndMaxima[0] <= 0f) minimaAndMaxima[0] = 1f;
        if (minimaAndMaxima[0] > minimaAndMaxima[1]) minimaAndMaxima[1] = minimaAndMaxima[0] + 1f;

        if (gustsAndGales.Length != 2) gustsAndGales = new float[2];
        if (gustsAndGales[0] <= 0f) gustsAndGales[0] = 1f;
        if (gustsAndGales[0] > gustsAndGales[1]) gustsAndGales[1] = gustsAndGales[0] + 1f;

        if (windSpeed == 0f) windSpeed = 1f;

        // Records the original location of the flag as loaded and stores it in the private float
        originalFlagRotation = flagObject.transform.localRotation.eulerAngles.z;

        // Starts the coroutine
        StartCoroutine(ChangeWind());

    }

    void FixedUpdate()
    {

        // Moves the flag's rotation vectors toward the target rotation
        if (rotateClockwise) flagObject.transform.Rotate(0f, 0f, windSpeed * Time.fixedDeltaTime);
        if (!rotateClockwise) flagObject.transform.Rotate(0f, 0f, -windSpeed * Time.fixedDeltaTime);

        if (flagObject.transform.localRotation.eulerAngles.z > originalFlagRotation + windIntensity) ChangeBool();
        if (flagObject.transform.localRotation.eulerAngles.z < originalFlagRotation - windIntensity) ChangeBool();

    }

    public IEnumerator ChangeWind()
    {

        // While the flag is not null
        while (flagObject != null)
        {

            // Sets random values for the wind's variability and speed
            windVariability = Random.Range(minimaAndMaxima[0], minimaAndMaxima[1]);
            windSpeed = Random.Range(gustsAndGales[0], gustsAndGales[1]);

            // Changes the flag's direction
            ChangeBool();

            // Refreshes the coroutine using the wind variability value
            yield return new WaitForSecondsRealtime(windVariability);

        }

    }

    public void ChangeBool()
    {

        // If the flag is rotating clockwise, it will reverse direction and vice versa
        if (rotateClockwise) rotateClockwise = false;
        else rotateClockwise = true;

    }

}
