/* Charlie Dye - ECT 4440 - 2026.02.06

This is the script for keeping track of current power-ups equipped */

using UnityEngine;

public class PowerUpBank : MonoBehaviour
{

    [Header("Boolean Variables")]
    [Tooltip("Is the \"Quad Cannons\" power-up applied?")] public bool qcCheck;
    [Tooltip("Is the \"Rapid Fire\" power-up applied?")] public bool rfCheck;
    [Tooltip("Is the \"Clear Steer\" power-up applied?")] public bool csCheck;
    [Tooltip("Is the \"Faster Bullets\" power-up applied?")] public bool fbCheck;
    [Tooltip("Is the \"Better Missiles\" power-up applied?")] public bool bmCheck;
    [Tooltip("Is the \"Powerful Thrusters\" power-up applied?")] public bool ptCheck;
    [Tooltip("Is the \"Shield\" power-up applied?")] public bool shCheck;

    void Start()
    {

        // Removes the power-up Booleans that are ticked true, if any are, as a preliminary measure
        RemoveAllBuffs();

    }

    public void RemoveAllBuffs()
    {

        // When the player loses a life, any buffs applied are lost
        if (qcCheck) qcCheck = false;
        if (rfCheck) rfCheck = false;
        if (csCheck) csCheck = false;
        if (fbCheck) fbCheck = false;
        if (bmCheck) bmCheck = false;
        if (ptCheck) ptCheck = false;
        if (shCheck) shCheck = false;

    }

}
