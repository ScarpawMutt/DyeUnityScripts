/* Charlie Dye - 2024.01.30

This is the script for the third person movement system */

using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    // Script for the camera
    private MoveCamera mc_ref;

    [Header("Rigidbody")]
    private Rigidbody miriam_RB;

    [Header("Movement Variables")]
    public float movement_speed;
    public float ground_drag;
    private Vector3 movement_direction_forward;
    private Vector3 movement_direction_sideways;
    private bool is_moving_forward;
    private bool is_moving_sideways;

    [Header("Orientation")]
    public Transform orientation;

    // Input variables
    private float input_horizontal;
    private float input_vertical;

    [Header("Ground Variables")]
    public float player_height;
    public LayerMask Ground;
    public bool can_move;
    private bool grounded;

    [Header("Gravity")]
    public float gravity_strength;
    private Vector3 downforce;

    // Position variable
    [HideInInspector] public Vector3 current_position;

    [Header("Debug Text")]
    public TextMeshProUGUI debug_text;

    [Header("Audio")]
    public AudioSource walking_audio;

    [Header("Keybinds")]
    public KeyCode move_up = KeyCode.W;
    public KeyCode move_left = KeyCode.A;
    public KeyCode move_down = KeyCode.S;
    public KeyCode move_right = KeyCode.D;

    // Start is called before the first frame update
    void Start()
    {

        // Enables movement right away
        can_move = true;

        // Fetches the script as a reference
        mc_ref = GetComponent<MoveCamera>();

        // Calls the rigibody and freezes rotation to prevent the capsule from falling over
        miriam_RB = GetComponent<Rigidbody>();
        miriam_RB.freezeRotation = true;

        // Calculates gravity (the Y-value should be negative!)
        downforce = new Vector3(0, gravity_strength, 0);

    }

    // Update is called once per frame
    void Update()
    {

        // Handles speed
        SpeedControl();

        // Updates the UI
        UpdateUI();

        // Checks if the rigidbody is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, player_height * 0.5f + 0.2f, Ground);

    }

    void FixedUpdate()
    {

        // Moves the player forward or backward
        if ((Input.GetKey(move_up) || Input.GetKey(move_down)) && !is_moving_sideways && can_move)
        {

            MovePlayerForward();
            is_moving_forward = true;

        }
        else
        {

            is_moving_forward = false;

        }

        // Moves the player left or right
        if ((Input.GetKey(move_left) || Input.GetKey(move_right)) && !is_moving_forward && can_move)
        {

            MovePlayerSideways();
            is_moving_sideways = true;

        }
        else
        {

            is_moving_sideways = false;

        }

        // Plays audio with pitch and volume randomized
        if ((is_moving_forward || is_moving_sideways))
        {

            walking_audio.enabled = true;
            walking_audio.mute = false;

            walking_audio.pitch = Random.Range(1f, 1.25f);
            walking_audio.volume = Random.Range(0.75f, 1.25f);

        }
        else
        {

            walking_audio.mute = true;

        }

        // Gets input for movement on the ground (WASD / Arrows)
        input_horizontal = Input.GetAxisRaw("Horizontal");
        input_vertical = Input.GetAxisRaw("Vertical");

        // Renews input for the player's position
        current_position = new (miriam_RB.position.x, miriam_RB.position.y, miriam_RB.position.z);

    }

    private void MovePlayerForward()
    {

        // Calculates the movement direction
        movement_direction_forward = orientation.forward * input_vertical;

        // Adds force to the rigidbody, whether on the ground...
        if (grounded)
        {

            miriam_RB.AddForce(10f * movement_speed * movement_direction_forward.normalized, ForceMode.Force);

        }
        else if (!grounded)
        {

            miriam_RB.AddForce(10f * movement_speed * movement_direction_forward.normalized + downforce, ForceMode.Force);

        }

    }

    private void MovePlayerSideways()
    {

        // Calculates the movement direction
        movement_direction_sideways = orientation.right * input_horizontal;

        // Adds force to the rigidbody, whether on the ground...
        if (grounded)
        {

            miriam_RB.AddForce(10f * movement_speed * movement_direction_sideways.normalized, ForceMode.Force);

        }
        else if (!grounded)
        {

            miriam_RB.AddForce(10f * movement_speed * movement_direction_sideways.normalized + downforce, ForceMode.Force);

        }

    }

    public void SpeedControl()
    {

        // Records the current velocity
        Vector3 velocity_flat = new (miriam_RB.velocity.x, 0, miriam_RB.velocity.z);

        // If said current velocity exceeds the movement speed value, this automatically limits it
        if (velocity_flat.magnitude > movement_speed)
        {

            Vector3 velocity_limited = velocity_flat.normalized * movement_speed;
            miriam_RB.velocity = new (velocity_limited.x, miriam_RB.velocity.y, velocity_limited.z);

        }

    }

    public void UpdateUI()
    {

        // Sends the rounded X- and Z-info to the text
        debug_text.text = "Location: (" + Mathf.Round(current_position.x).ToString() + ", " + Mathf.Round(current_position.z).ToString() + ")";
        debug_text.text += "\nVelocity: (" + Mathf.Round(miriam_RB.velocity.x * 10f).ToString() + ", " + Mathf.Round(miriam_RB.velocity.z * 10f).ToString() + ")";
        debug_text.text += "\nCamera Orientation: " + mc_ref.rotation_status;

    }

}
