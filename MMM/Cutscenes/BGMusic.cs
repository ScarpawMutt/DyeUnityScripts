using UnityEngine;

public class BGMusic : MonoBehaviour
{

    public static BGMusic BGInstance;

    private void Awake()
    {

        BGInstance = this;

        if (BGInstance != null && BGInstance != this)
        {

            Destroy(gameObject);
            return;

        }

        DontDestroyOnLoad(this);

    }

}
