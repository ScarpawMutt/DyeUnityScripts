using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource Audio;

    public AudioClip Click;

    public static SFXManager SFXInstance;

    private void Awake()
    {

        if(SFXInstance != null && SFXInstance != this)
        {

            Destroy(this.gameObject);
            return;

        }

        SFXInstance = this;
        DontDestroyOnLoad(this);

    }
}
