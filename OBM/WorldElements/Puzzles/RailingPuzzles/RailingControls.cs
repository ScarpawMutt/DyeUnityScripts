/* Charlie Dye, PACE Team - 2025.12.03

This is the master script for the railing puzzle gadgets */

using UnityEngine;

public class RailingControls : MonoBehaviour
{

    [Header("Child Script References and Arrays")]
    [Tooltip("The script for the analog clock.")] public AnalogClock acReference;
    [Tooltip("The script for the text on the panel screen.")] public PanelText ptReference;
    [Tooltip("The script for the flipper switches.")] public FlipperBehavior[] fbReferences;
    [Tooltip("The script for the gauge needles.")] public GaugeBehavior[] gbReferences;
    [Tooltip("The script for the array of colored lights.")] public PsychedeilcLighting[] plReferences;

    void Start()
    {

        // If the analog clock or panel text script is left null, then this script will self-destruct
        if (acReference == null || ptReference == null) Destroy(this);

        // Does the same for the arrays by scanning each element
        for (int i = 0; i < fbReferences.Length; i++)
        {

            if (fbReferences[i] == null)
            {

                Destroy(this);
                break;

            }

        }

        for (int j = 0; j < gbReferences.Length; j++)
        {

            if (gbReferences[j] == null)
            {

                Destroy(this);
                break;

            }

        }

        for (int k = 0; k < plReferences.Length; k++)
        {

            if (plReferences[k] == null)
            {

                Destroy(this);
                break;

            }

        }

    }

    public void SetOffInstruments()
    {

        // Activates the panel's instruments by referencing their local Boolean variables
        acReference.canInvert = true;
        ptReference.canAnimate = true;

        for (int l = 0; l < fbReferences.Length; l++)
        {

            fbReferences[l].canSwitch = true;

        }

        for (int m = 0; m < gbReferences.Length; m++)
        {

            gbReferences[m].canInvert = true;

        }

        for (int n = 0; n < plReferences.Length; n++)
        {

            plReferences[n].canSwitch = true;

        }

    }

    public void StopInstruments()
    {

        // Deactivates the panel's instruments by referencing their local Boolean variables
        acReference.canInvert = false;
        ptReference.canAnimate = false;

        for (int o = 0; o < fbReferences.Length; o++)
        {

            fbReferences[o].canSwitch = false;

        }

        for (int p = 0; p < gbReferences.Length; p++)
        {

            gbReferences[p].canInvert = false;

        }

        for (int q = 0; q < plReferences.Length; q++)
        {

            plReferences[q].canSwitch = false;

            // Forcefully switches all lights on
            if (!plReferences[q].associatedLight.enabled) plReferences[q].associatedLight.enabled = true;

        }

    }

}
