using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    public void GoToMainMenu()
    {

        SceneManager.LoadScene("Main_Menu");
        SFXManager.SFXInstance.Audio.PlayOneShot(SFXManager.SFXInstance.Click);

    }

    public void GoToCutscene()
    {

        SceneManager.LoadScene("CutsceneScene");
        SFXManager.SFXInstance.Audio.PlayOneShot(SFXManager.SFXInstance.Click);

    }

    public void GoToControls()
    {

        SceneManager.LoadScene("Controls_Menu");
        SFXManager.SFXInstance.Audio.PlayOneShot(SFXManager.SFXInstance.Click);

    }

    public void GoToCredits()
    {

        SceneManager.LoadScene("Credits_Menu");
        SFXManager.SFXInstance.Audio.PlayOneShot(SFXManager.SFXInstance.Click);

    }

}
