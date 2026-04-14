/* Charlie Dye - 2024.04.16

This is the script for dialogue text */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueText : MonoBehaviour
{

    [Header("Script References")]
    public PlayerMovement pm_ref;
    public SpriteBehavior sb_ref;

    [Header("String Messages")]
    [TextArea(1, 3)] public string[] miriam_interaction;
    public string[] name_list;
    private string character_name;
    public int array_index_value = 0;

    [Header("UI Attributes")]
    public TextMeshProUGUI text_box;
    public TextMeshProUGUI name_box;
    public TextMeshProUGUI message_box;
    public float message_duration;

    [Header("Interaction Keys")]
    public KeyCode interaction_key = KeyCode.E;
    public KeyCode abort_dialogue_key = KeyCode.Escape;

    // Boolean values
    [HideInInspector] public bool can_interact;
    [HideInInspector] public bool is_speaking;
    public bool is_goatcu;

    void Start()
    {

        // Clears the text box of any text
        text_box.text = null;
        name_box.text = null;

        // Sets the character name according to the boolean values
        if (is_goatcu) character_name = "Goatcu";
        else if (!is_goatcu) character_name = "Miner";

    }

    void Update()
    {

        // Determines whether to activate or deactivate text depending on if the player is "reading" or not
        if (can_interact && Input.GetKeyDown(interaction_key) && (!sb_ref.is_moving_up && !sb_ref.is_moving_down && !sb_ref.is_moving_left && !sb_ref.is_moving_right))
        {

            if (!is_speaking)
            {

                DisplayText();

            }

        }

        // If the interaction key is pressed, the array value will increment by one
        if (is_speaking && Input.GetKeyDown(interaction_key))
        {

            array_index_value++;

        }

        // The game uses this info to display the appropriate names and dialogue, depending on if the value is in the array's bounds or not
        if (is_speaking)
        {

            try
            {

                text_box.text = miriam_interaction[array_index_value - 1];
                name_box.text = name_list[array_index_value - 1];

            }
            catch (IndexOutOfRangeException)
            {

                ExitText();

            }

        }

        // If the player presses the dialogue abortion key, the dialogue will close immediately
        if (Input.GetKeyDown(abort_dialogue_key))
        {

            ExitText();

        }

    }

    void OnTriggerEnter(Collider speaker)
    {

        if (speaker.CompareTag("Miriam") || speaker.name == "Player")
        {

            // Enables interaction
            can_interact = true;

            // Prints a message to the console for a specified amount of time if the player is not "reading"
            if (!is_speaking && message_box.text == null)
            {

                message_box.text = "Oh, here's " + character_name + ". Press " + interaction_key.ToString() + " to interact with them.";

            }

        }

    }

    void OnTriggerExit(Collider speaker)
    {

        if (speaker.CompareTag("Miriam") || speaker.name == "Player")
        {

            // Disables interaction
            can_interact = false;

            // Resets the console message
            message_box.text = null;

        }

    }

    // Displays text
    public void DisplayText()
    {

        // Disables movement
        pm_ref.can_move = false;

        // Enables the reading boolean
        is_speaking = true;

    }

    // Undoes text
    public void ExitText()
    {

        // Reenables movement
        pm_ref.can_move = true;

        // Disables the reading boolean
        is_speaking = false;

        // Reverts the UI text to a null value
        text_box.text = null;
        name_box.text = null;

        // Reverts the array index value to 0
        array_index_value = 0;

        // Sends info to the UI message text for a specified amount of time
        message_box.text = "That's all from " + character_name + ". The conversation is over.";
        Invoke(nameof(ResetMessageBox), message_duration);

    }

    // Resets the message box
    public void ResetMessageBox()
    {

        message_box.text = null;

    }

}
