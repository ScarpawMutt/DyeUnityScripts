/* Charlie Dye - 2024.04.23

This is the script for randomly generated NPC movement */

using UnityEngine;
using UnityEngine.UI;

public class TitleBackground : MonoBehaviour
{

    public RectTransform title_transform;

    // Start is called before the first frame update
    void Start()
    {

        title_transform.anchorMin = new Vector2(0, 0);
        title_transform.anchorMax = new Vector2(1, 1);

    }

}
