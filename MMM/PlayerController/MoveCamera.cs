/* Charlie Dye - 2024.02.01

This is the script used to control the movement of the camera */

using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    [Header("Player Axis")]
    public GameObject player_axis;

    [Header("Rotation")]
    public float degrees_of_rotation;
    public float rotation_speed;
    private Quaternion target_rotation;

    // Bools
    private bool ready_to_rotate;

    [Header("Delay Time")]
    public float seconds_until_reset;

    // String variable
    [HideInInspector] public string rotation_status;

    // Keybinds
    public KeyCode move_camera_right = KeyCode.RightArrow;
    public KeyCode move_camera_left = KeyCode.LeftArrow;

    // Sets the quaternion
    private void Awake()
    {
        
        target_rotation = transform.rotation;

    }

    // Allows for rotation right away
    void Start()
    {

        ready_to_rotate = true;

    }

    void Update()
    {

        // Updates the UI
        if (ready_to_rotate) rotation_status = "Ready!";
        else rotation_status = "Charging...";

    }

    void FixedUpdate()
    {
        
        // Rotates the camera right by a specified amount of degrees
        if (Input.GetKey(move_camera_right) && ready_to_rotate)
        {

            // Rotates the camera by the specified degrees of rotation
            target_rotation *= Quaternion.AngleAxis(degrees_of_rotation, transform.up);

            // Disables rotating in the same direction again
            ready_to_rotate = false;

            // Enables a reversion of the camera after a specified amount of time
            Invoke(nameof(ResetRotationAbility), seconds_until_reset);

        }

        // Rotates the camera left by a specified amount of degrees
        if (Input.GetKey(move_camera_left) && ready_to_rotate)
        {

            // Rotates the camera by the specified degrees of rotation
            target_rotation *= Quaternion.AngleAxis(-degrees_of_rotation, transform.up);

            // Disables rotating in the same direction again
            ready_to_rotate = false;

            // Enables a reversion of the camera after a specified amount of time
            Invoke(nameof(ResetRotationAbility), seconds_until_reset);

        }

        // Quaternion equation for above
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, rotation_speed * Time.deltaTime);

    }

    void ResetRotationAbility()
    {

        // Re-enables looking the opposite way
        ready_to_rotate = true;

    }

}