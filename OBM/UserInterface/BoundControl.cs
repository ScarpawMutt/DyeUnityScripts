/* Charlie Dye, PACE Team - 2024.02.13

This is the script that keeps props from permanently falling out of the world */

using UnityEngine;

public class BoundControl : MonoBehaviour
{

    [Header("Rigidbody Variable")]
    [Tooltip("The rigid body to keep track of.")] public Rigidbody propRigidbody;

    // Vector for prop positions
    private Vector3 startingPosition;

    [Header("Deadline Levels")]
    public float boundDeadline;

    void Start()
    {

        // Fetches the rigidbody
        propRigidbody = GetComponent<Rigidbody>();

        // Marks down the starting coordinates as original
        startingPosition = new Vector3(propRigidbody.transform.position.x, propRigidbody.transform.position.y, propRigidbody.transform.position.z);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // If the rigidbody's y-level falls below a certain elevation
        if (propRigidbody.transform.position.y < boundDeadline)
        {

            // Velocity is set to zero
            propRigidbody.velocity = Vector3.zero;

            // The object is teleported back to where it was loaded in the world initially
            propRigidbody.transform.SetPositionAndRotation(startingPosition, Quaternion.identity);

        }

    }

}
