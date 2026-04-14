/* Charlie Dye - 2024.02.27

This is the script for sprite behavior */

using UnityEngine;

public class SpriteBehavior : MonoBehaviour
{

    [Header("Script Reference")]
    public PlayerMovement pm_ref;

    [Header("Camera")]
    public Camera player_camera;

    [Header("Pivot Point")]
    public Transform sprite_location;

    [Header("Sprite Array")]
    public GameObject[] character_sprites;

    [Header("Rigidbody")]
    public Rigidbody subject_rb;

    [Header("Keybinds")]
    public KeyCode key_up = KeyCode.W;
    public KeyCode key_down = KeyCode.S;
    public KeyCode key_left = KeyCode.A;
    public KeyCode key_right = KeyCode.D;

    // Boolean variables
    [HideInInspector] public bool is_moving_up;
    [HideInInspector] public bool is_moving_down;
    [HideInInspector] public bool is_moving_left;
    [HideInInspector] public bool is_moving_right;
    [HideInInspector] public bool is_facing_up;
    [HideInInspector] public bool is_facing_down;
    [HideInInspector] public bool is_facing_left;
    [HideInInspector] public bool is_facing_right;
    [HideInInspector] public bool is_neutral;
    public bool can_change_values;

    // Start is called before the first frame update
    void Start()
    {

        ResetAnimations();
        ResetBooleans(true);
        is_neutral = true;
        character_sprites[0].GetComponent<SpriteRenderer>().enabled = true;

    }

    // Update is called once per frame
    void Update()
    {

        // Billboards the sprite
        sprite_location.transform.rotation = player_camera.transform.rotation;

        // This section is for the player controller only
        if(gameObject.name == "Player" || gameObject.CompareTag("Miriam"))
        {

            if (pm_ref.can_move)
            {

                // Controls the boolean values through key inputs
                if (Input.GetKey(key_up) && can_change_values)
                {

                    ResetBooleans(true);
                    is_moving_up = true;

                    ResetAnimations();
                    character_sprites[4].GetComponent<SpriteRenderer>().enabled = true;

                }
                else if (Input.GetKey(key_down) && can_change_values)
                {

                    ResetBooleans(true);
                    is_moving_down = true;

                    ResetAnimations();
                    character_sprites[5].GetComponent<SpriteRenderer>().enabled = true;

                }
                else if (Input.GetKey(key_left) && can_change_values)
                {

                    ResetBooleans(true);
                    is_moving_left = true;

                    ResetAnimations();
                    character_sprites[6].GetComponent<SpriteRenderer>().enabled = true;

                }
                else if (Input.GetKey(key_right) && can_change_values)
                {

                    ResetBooleans(true);
                    is_moving_right = true;

                    ResetAnimations();
                    character_sprites[7].GetComponent<SpriteRenderer>().enabled = true;

                }

                // Once any of the keys are released, the neutral boolean activates
                if (Input.GetKeyUp(key_up) || Input.GetKeyUp(key_down) || Input.GetKeyUp(key_left) || Input.GetKeyUp(key_right))
                {

                    ResetBooleans(true);
                    is_neutral = true;

                    if (Input.GetKeyUp(key_up))
                    {

                        ResetAnimations();
                        is_facing_up = true;

                    }
                    else if (Input.GetKeyUp(key_down))
                    {

                        ResetAnimations();
                        is_facing_down = true;

                    }
                    else if (Input.GetKeyUp(key_left))
                    {

                        ResetAnimations();
                        is_facing_left = true;

                    }
                    else if (Input.GetKeyUp(key_right))
                    {

                        ResetAnimations();
                        is_facing_right = true;

                    }

                }

                // Basic logic for permitting boolean values to change
                if (is_neutral) can_change_values = true;
                else can_change_values = false;

            }

            // Changes sprite appearance based on above booleans
            if (is_facing_up) character_sprites[0].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_facing_down) character_sprites[1].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_facing_left) character_sprites[2].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_facing_right) character_sprites[3].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_moving_up) character_sprites[4].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_moving_down) character_sprites[5].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_moving_left) character_sprites[6].GetComponent<SpriteRenderer>().enabled = true;
            else if (is_moving_right) character_sprites[7].GetComponent<SpriteRenderer>().enabled = true;

        }

    }

    public void ResetAnimations()
    {

        for (int i = 0; i < character_sprites.Length; i++)
        {

            character_sprites[i].GetComponent<SpriteRenderer>().enabled = false;

        }

    }

    public void ResetBooleans(bool reset_faces_too)
    {

        is_moving_up = false;
        is_moving_down = false;
        is_moving_left = false;
        is_moving_right = false;
        is_neutral = false;

        if (reset_faces_too)
        {

            is_facing_up = false;
            is_facing_down = false;
            is_facing_left = false;
            is_facing_right = false;

        }

    }

}
