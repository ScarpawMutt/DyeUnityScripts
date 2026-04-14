/* Charlie Dye - 2024.02.27

This is the script for randomly generated NPC movement */

using UnityEngine;

public class RandomMovement : MonoBehaviour
{

    [Header("Script References")]
    private SpriteBehavior sprite_ref;

    // RNG variable
    private int random_movement_agent;

    [Header("Frequency of Events")]
    public float frequency_variable;

    void Start()
    {

        // Fetches the script
        sprite_ref = GetComponent<SpriteBehavior>();

        // Repeatedly randomizes the movement agent if the character is not busy talking
        InvokeRepeating(nameof(RandomizeMovement), 0f, frequency_variable);

    }

    public void RandomizeMovement()
    {

        // Resets stationary animations
        sprite_ref.ResetAnimations();
        
        // Randomly selects a value between 0 and 4
        random_movement_agent = Random.Range(0, 4);

        // Changes the sprite displayed according to the number chosen
        if (random_movement_agent == 0) sprite_ref.character_sprites[0].GetComponent<SpriteRenderer>().enabled = true;
        else if (random_movement_agent == 1) sprite_ref.character_sprites[1].GetComponent<SpriteRenderer>().enabled = true;
        else if (random_movement_agent == 2) sprite_ref.character_sprites[2].GetComponent<SpriteRenderer>().enabled = true;
        else if (random_movement_agent == 3) sprite_ref.character_sprites[3].GetComponent<SpriteRenderer>().enabled = true;

    }

}
