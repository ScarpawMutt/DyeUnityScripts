/* Charlie Dye, PACE Team - 2024.10.01

This is the script for the gears in the elevator */

using UnityEngine;

public class GearBehavior : MonoBehaviour
{

    [Header("Gear Arrays and Objects")]
    public GameObject[] gearTrain;
    public GameObject centralGear;
    public GameObject gearAxle;

    [Header("Audio Source")]
    public AudioSource gearNoise;

    // Float variable
    [HideInInspector] public float gearRotationSpeed;

    // Boolean variables
    [HideInInspector] public bool executeAnimation;
    [HideInInspector] public bool reverseRotation;
    [HideInInspector] public bool audioIsPlaying;

    void FixedUpdate()
    {

        // If the animation boolean returns true, then the animation and audio will play in FixedUpdate()
        if (executeAnimation)
        {

            GearAnimation();

            if (!audioIsPlaying)
            {

                gearNoise.Play();
                audioIsPlaying = true;

            }

        }
        else audioIsPlaying = false;

    }

    public void GearAnimation()
    {

        // Logic for the entire gear train moving in one direction
        if (!reverseRotation)
        {

            /* If the gear's array value is even, then they will rotate in one direction;
            otherwise, it will rotate in the other direction */
            for (int i = 0; i < gearTrain.Length; i++)
            {

                if (i % 2 == 0) gearTrain[i].transform.Rotate(Vector3.forward * (gearRotationSpeed * Time.deltaTime));
                else gearTrain[i].transform.Rotate(Vector3.back * (gearRotationSpeed * Time.deltaTime));


            }

            // Rotation logic for the central gear and shaft
            centralGear.transform.Rotate(Vector3.left * (gearRotationSpeed * Time.deltaTime));
            gearAxle.transform.Rotate(Vector3.up * (gearRotationSpeed * Time.deltaTime));

        }
        // Logic for the entire gear train moving in the opposite direction
        else
        {

            /* If the gear's array value is even, then they will rotate in one direction;
            otherwise, it will rotate in the other direction */
            for (int i = 0; i < gearTrain.Length; i++)
            {

                if (i % 2 == 0) gearTrain[i].transform.Rotate(Vector3.back * (gearRotationSpeed * Time.deltaTime));
                else gearTrain[i].transform.Rotate(Vector3.forward * (gearRotationSpeed * Time.deltaTime));

            }

            // Rotation logic for the central gear and shaft
            centralGear.transform.Rotate(Vector3.right * (gearRotationSpeed * Time.deltaTime));
            gearAxle.transform.Rotate(Vector3.down * (gearRotationSpeed * Time.deltaTime));

        }

    }

}