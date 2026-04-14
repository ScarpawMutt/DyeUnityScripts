/* Charlie Dye, PACE Team - 2024.09.05

This is the script for the flashlight's light */

using UnityEngine;

public class FlashlightBehavior : MonoBehaviour
{

    // Script reference
    private GroundedCheck gcReference;

    [Header("Flashlight Light")]
    [Tooltip("The light source attached to this object.")] public Light bulbLight;

    // Start is called before the first frame update
    void Start()
    {

        // References the script
        gcReference = GetComponent<GroundedCheck>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // If the extinguisher is suspended in midair, then the particle system plays
        if (!gcReference.isGrounded) bulbLight.enabled = true;
        else bulbLight.enabled = false;

    }

}
