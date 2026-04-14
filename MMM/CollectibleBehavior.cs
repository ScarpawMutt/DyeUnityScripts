/* Charlie Dye - 2023.02.06

This is the script for the Astrolite collectible prefab */

using System.Collections;
using UnityEngine;
using TMPro;

public class CollectibleBehavior : MonoBehaviour
{

    [Header("Script Reference")]
    public static CollectibleBehavior instance;
    public PlayerInventory pi_reference;

    [Header("Camera")]
    public Camera player_camera;

    [Header("Pivot Point")]
    public Transform sprite_location;

    // Boolean variable
    public bool can_collect = false;

    [Header("Keybind")]
    public KeyCode collection_key = KeyCode.E;

    [Header("UI Attributes")]
    public TextMeshProUGUI message_box;

    void Awake()
    {

        // The class references itself when loading
        instance = this;

    }

    void Update()
    {

        // Billboards the sprite
        sprite_location.transform.rotation = player_camera.transform.rotation;

        // Collection system
        if (can_collect && Input.GetKeyDown(collection_key))
        {

            // Initiates the other script's method
            pi_reference.OnCollected();

            // Despawns the collectible
            Destroy(gameObject);

        }

    }

    // If the player enters the area, this will trigger
    void OnTriggerStay(Collider avatar)
    {

        if (avatar.CompareTag("Miriam") || avatar.name == "Player")
        {

            // Enables collection
            can_collect = true;

            // Prints a message
            message_box.text = "I'm standing within collection distance of some Astrolite! Press " + collection_key.ToString() + " to collect it.";

        }

    }

    // If the player exits the area, this will trigger
    void OnTriggerExit(Collider avatar)
    {

        if (avatar.CompareTag("Miriam") || avatar.name == "Player")
        {

            // Disables collection
            can_collect = false;

            // Resets the message
            message_box.text = null;

        }

    }

}