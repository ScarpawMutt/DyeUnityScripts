/* Charlie Dye - 2024.04.23

This is the script for randomly generated NPC movement */

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneBehavior : MonoBehaviour
{

    // VideoPlayer variable
    private VideoPlayer cutscene_player;

    [Header("Keybind")]
    public KeyCode skip_key = KeyCode.Mouse0;

    void Start()
    {

        cutscene_player = GetComponent<VideoPlayer>();

        Invoke(nameof(DisableCutscene), (float)cutscene_player.length + 1.0f);

    }

    private void Update()
    {
        
        if (Input.GetKeyDown(skip_key))
        {

            DisableCutscene();

        }

    }

    public void DisableCutscene()
    {

        cutscene_player.enabled = false;
        Invoke(nameof(LoadGameplay), 1.0f);

    }

    public void LoadGameplay()
    {

        SceneManager.LoadScene("StitchedScene");

    }

}
