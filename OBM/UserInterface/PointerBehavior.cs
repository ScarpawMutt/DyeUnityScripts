/* Charlie Dye, PACE Team - 2025.09.11

This is the script for the pointer at the player's feet */

using UnityEngine;

public class PointerBehavior : MonoBehaviour
{

    [Header("GameObjects")]
    [Tooltip("The arrow that this script will control.")] public GameObject pointerArrow;
    [Tooltip("The interactive object that the pointer is associated with.")] public GameObject pointerTarget;

    [Header("Transform Target")]
    [Tooltip("The point in local space that the pointer snaps to.")] public Transform pointerFollowMe;

    [Header("Mesh Variable")]
    public MeshRenderer pointerMesh;

    [Header("Float Variables")]
    [Tooltip("The distance above the target object that the pointer floats.")] public float distanceAboveTarget;
    [Tooltip("The rate at which the pointer rotates on its axis.")] public float spinRate;

    [Header("Boolean Variable")]
    [Tooltip("Should the pointer be visible or not?")] public bool renderPointer = false;
    private bool pointerIsActive = false;
    private bool pointerIsHidden = false;
    public bool hidePointerPermanently = false;

    void Start()
    {

        // If specific floats are set incorrectly, then this will correct them automatically
        if (distanceAboveTarget <= 0f) distanceAboveTarget = 0.1f;
        if (spinRate == 0f) spinRate = 1f;

        // Correctly establishes the private Boolean variables
        if (!pointerIsActive) pointerIsActive = true;
        if (pointerIsHidden) pointerIsHidden = false;

    }

    void FixedUpdate()
    {

        // If the pointer is relevant
        if (!hidePointerPermanently)
        {

            // If the pointer must be rendered
            if (renderPointer)
            {

                // Makes the pointer float above its target
                FloatOnTarget();

                // Rotates the pointer
                RotatePointer();

            }

        }
        else renderPointer = false;

        // Controls the pointer visibility
        PointerControl();

    }

    public void FloatOnTarget()
    {

        // Establishes the target transform that the pointer must follow
        pointerFollowMe.localPosition = new Vector3(pointerTarget.transform.localPosition.x, pointerTarget.transform.localPosition.y + distanceAboveTarget,
            pointerTarget.transform.localPosition.z);

        // Snaps the pointer to its target
        pointerArrow.transform.position = Vector3.MoveTowards(pointerArrow.transform.position, pointerFollowMe.transform.position, 1f);

    }

    public void PointerControl()
    {

        // If the pointer is active but must disappear
        if (!renderPointer && pointerIsActive)
        {

            // Mesh visibility is controlled via the kill bools
            pointerMesh.enabled = false;
            pointerIsActive = false;
            pointerIsHidden = true;

        }
        // If the pointer is inactive but must appear
        else if (renderPointer && pointerIsHidden)
        {

            // Mesh visibility is controlled via the kill bools
            pointerMesh.enabled = true;
            pointerIsActive = true;
            pointerIsHidden = false;

        }

    }

    public void RotatePointer()
    {

        // Uses rotation to create a spinning effect above the object
        pointerArrow.transform.Rotate(spinRate * Time.fixedDeltaTime * Vector3.up, Space.Self);

    }

    public void DisableActivePointer()
    {

        // If the pointer is still relevant, then it will disappear (this is triggered by the fire extinguisher's interactable listeners)
        if(!hidePointerPermanently && renderPointer) renderPointer = false;

    }

    public void AmendActivePointer()
    {

        // If the pointer is still relevant, then it will reappear (this is triggered by the fire extinguisher's interactable listeners)
        if (!hidePointerPermanently && !renderPointer) renderPointer = true;

    }

}
