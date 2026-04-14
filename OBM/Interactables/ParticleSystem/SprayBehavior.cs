/* Charlie Dye, PACE Team - 2024.09.05

This is the script for the fire extinguisher particle system */

using UnityEngine;

public class SprayBehavior : MonoBehaviour
{

    // Grounded script reference
    private GroundedCheck gcReference;

    [Header("GameObject Variable")]
    [Tooltip("The nozzle GameObject on the spray can.")] public GameObject sprayNozzle;
    private Vector3 originalNozzlePosition;
    private Vector3 depressedNozzlePosition;

    [Header("Float Variable")]
    [Tooltip("The distance the nozzle travels downward when the object is in use.")] public float depressedNozzleTravel;

    [Header("Foam Particle System")]
    [Tooltip("The paint particles emitted from the nozzle opening.")] public ParticleSystem projectileSpray;

    [Header("Audio Source")]
    [Tooltip("The noise that plays when in use.")] public AudioSource spraySound;

    // Boolean variables
    private bool resetGrounding = false;
    private bool resetNongrounding = false;

    // Start is called before the first frame update
    void Start()
    {

        // References the script
        gcReference = GetComponent<GroundedCheck>();

        // Records the original and depressed nozzle positions
        originalNozzlePosition = sprayNozzle.transform.localPosition;
        depressedNozzlePosition = new(originalNozzlePosition.x, originalNozzlePosition.y, originalNozzlePosition.z - depressedNozzleTravel);
        
    }

    void FixedUpdate()
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

    void ActionControl()
    {

        // If the spray can is not grounded
        if (!gcReference.isGrounded)
        {

            // Plays the foam animation and noise
            projectileSpray.Play();
            spraySound.Play();

            // Sets the position of the nozzle
            sprayNozzle.transform.localPosition = depressedNozzlePosition;

        }
        // If the spray can is grounded
        else
        {

            // Stops the foam animation and noise
            projectileSpray.Stop();
            spraySound.Stop();
            
            // Sets the position of the nozzle
            sprayNozzle.transform.localPosition = originalNozzlePosition;

        }

    }

}
