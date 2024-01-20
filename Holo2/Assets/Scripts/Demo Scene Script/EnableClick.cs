using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class EnableClick : MonoBehaviour
{

    private bool clicked = false;
    public GameObject magnet1;
    public GameObject magnet2;

    /*
     * Function that enables the clicking action, disabling scaling, translation and rotation of objects
     */
    public void EnableClicking()
    {
        clicked = !clicked;
        if (clicked)
        {
            this.GetComponent<Image>().color = new Color(0f, 0.68f, 0.812f);
            magnet1.GetComponent<ManipulationHandler>().enabled = false;
            magnet1.GetComponent<ObjectManipulator>().enabled = false;
            magnet1.GetComponent<NearInteractionGrabbable>().enabled = false;
            magnet2.GetComponent<ManipulationHandler>().enabled = false;
            magnet2.GetComponent<ObjectManipulator>().enabled = false;
            magnet2.GetComponent<NearInteractionGrabbable>().enabled = false;
        } else
        {
            this.GetComponent<Image>().color = new Color(0f, 0.78f, 1f);
            magnet1.GetComponent<ManipulationHandler>().enabled = true;
            magnet1.GetComponent<ObjectManipulator>().enabled = true;
            magnet1.GetComponent<NearInteractionGrabbable>().enabled = true;
            magnet2.GetComponent<ManipulationHandler>().enabled = true;
            magnet2.GetComponent<ObjectManipulator>().enabled = true;
            magnet2.GetComponent<NearInteractionGrabbable>().enabled = true;
        }
    }
}
