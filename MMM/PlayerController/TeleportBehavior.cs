/* Charlie Dye - 2024.01.30

This is the script for the third person movement system */

using UnityEngine;

public class TeleportBehavior : MonoBehaviour
{

    [Header("Player Controller")]
    public GameObject object_to_teleport;

    [Header("Location to Teleport to")]
    public Vector3 teleport_target;

    private void OnTriggerEnter(Collider avatar)
    {
        
        if (avatar.CompareTag("Miriam"))
        {

            object_to_teleport.transform.position = teleport_target;

        }    

    }

}
