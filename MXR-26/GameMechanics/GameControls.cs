/* Charlie Dye - ECT 4440 - 2026.01.22

This is the script for determining which control set the game should use */

using UnityEngine;

public class GameControls : MonoBehaviour
{

    [Header("Boolean Variable")]
    [Tooltip("Should the game be controlled with a VR setup (true) or a standard keyboard (false)?")] public bool useXRControls;

}
