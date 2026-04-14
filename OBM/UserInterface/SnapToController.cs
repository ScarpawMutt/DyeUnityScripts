/* Charlie Dye, PACE Team - 2024.03.06

This is the script to make the control panel trigger stick to a controller */

using UnityEngine;

public class SnapToController : MonoBehaviour
{

    [Header("Target Position for Trigger")]
    public Transform targetPosition;

    [Header("Other Collider")]
    public GameObject[] otherColliders;

    void FixedUpdate()
    {

        // Instructs the gameObject to clip onto the transforms of another
        gameObject.transform.SetPositionAndRotation(Vector3.MoveTowards(gameObject.transform.position, targetPosition.transform.position, 1f), targetPosition.rotation);

        // Ignores other colliders' collisions
        for (int i = 0; i < otherColliders.Length; i++)
        {

            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), otherColliders[i].GetComponent<Collider>());

        }
        
    }

}
