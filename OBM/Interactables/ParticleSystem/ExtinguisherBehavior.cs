/* Charlie Dye, PACE Team - 2024.09.05

This is the script for the fire extinguisher particle system */

using UnityEngine;

public class ExtinguisherBehavior : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    private GroundedCheck gcReference;

    [Header("GameObject Arrays")]
    [Tooltip("An array that stores the models of the extinguisher's hose. Should ideally contain two elements.")] public GameObject[] hosePositions;
    [Tooltip("An array that stores the models of the extinguisher's lever. Should ideally contain two elements.")] public GameObject[] leverPositions;

    [Header("Foam Particle System")]
    [Tooltip("The foam particles emitted from the hose opening.")] public ParticleSystem projectileSpray;

    [Header("Audio Source")]
    [Tooltip("The noise that plays when in use.")] public AudioSource spraySound;

    // Boolean variables
    private bool resetGrounding = false;
    private bool resetNongrounding = false;

    void Awake()
    {

        // References the script
        gcReference = GetComponent<GroundedCheck>();

        // If the private script variable returns null, then this script will self-destruct
        if (gcReference == null) Destroy(this);

    }

    void Start()
    {

        // Retracts the extinguisher's hose
        hosePositions[0].SetActive(true);
        hosePositions[1].SetActive(false);
        leverPositions[0].SetActive(true);
        leverPositions[1].SetActive(false);

        // Disables foam and sound directly
        projectileSpray.Stop();
        spraySound.Stop();

    }

    void FixedUpdate()
    {

        // If the player is at or heading for the fifth floor
        if (ebReference.arrayIndexer == 5)
        {

            // If the grounded bool returns true, then the resulting method will be called once
            if (gcReference.isGrounded && !resetGrounding)
            {

                ActionControl();
                resetGrounding = true;
                resetNongrounding = false;

            }
            // The same occurs if the grounded bool returns false as well
            if (!gcReference.isGrounded && !resetNongrounding)
            {

                ActionControl();
                resetGrounding = false;
                resetNongrounding = true;

            }

        }

    }

    void ActionControl()
    {

        // If the extinguisher is not grounded
        if (!gcReference.isGrounded)
        {

            // Plays the foam animation and noise
            projectileSpray.Play();
            spraySound.Play();

            // Changes "positions" of the lever and hose, simulating a real life extinguisher
            hosePositions[0].SetActive(false);
            hosePositions[1].SetActive(true);
            leverPositions[0].SetActive(false);
            leverPositions[1].SetActive(true);
            
        }
        // If the extinguisher is grounded
        else
        {

            // Stops the foam animation and noise
            projectileSpray.Stop();
            spraySound.Stop();

            // Changes "positions" of the lever and hose, simulating a real life extinguisher
            hosePositions[0].SetActive(true);
            hosePositions[1].SetActive(false);
            leverPositions[0].SetActive(true);
            leverPositions[1].SetActive(false);

        }

    }

}
