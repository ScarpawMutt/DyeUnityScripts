/* Charlie Dye - 2024.02.06

This is the script for the Astrolite collection system */

using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{

    [Header("Script References")]
    public static PlayerInventory instance;
    public CollectibleBehavior cb_ref;

    [Header("Amount of Astrolite")]
    public int astrolite_gained;
    private int astrolite_owned;
    public int maximum_amount;

    [Header("UI Attributes")]
    public TextMeshProUGUI collection_text;
    public TextMeshProUGUI message_box;
    public float message_duration;

    [Header("Audio")]
    public AudioSource pickup_audio;

    void Awake()
    {

        // The class references itself when loading
        instance = this;

    }

    void Start()
    {

        // Automatically sets the initial amount to zero
        astrolite_owned = 0;

        // Relays this information to the UI text
        collection_text.text = astrolite_owned.ToString();

    }

    public void OnCollected()
    {

        // Plays audio
        pickup_audio.Play();

        // Increments the value by a specified amount
        astrolite_owned += astrolite_gained;

        // Updates the counter accordindly
        collection_text.text = astrolite_owned.ToString();

        // Prints a message to the console for a specified amount of time
        message_box.text = "Astrolite! Way to go!";
        Invoke(nameof(ResetMessageText), message_duration);

    }

    public void ResetMessageText()
    {

        message_box.text = null;

    }

}