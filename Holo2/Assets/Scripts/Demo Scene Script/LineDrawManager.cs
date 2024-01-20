using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class LineDrawManager : MonoBehaviour
    {
        public Canvas controlCanvas;
        public Canvas drawCanvas;
        public Canvas shareCanvas;
        public float targetOpacity = 0.5f;
        public GameObject selectButton;
        public GameObject exitButton;
        public GameObject stopDarwButton;
        public GameObject deleteButton;



        // Start is called before the first frame update
        void Start()
        {
            controlCanvas.gameObject.SetActive(true);
            drawCanvas.gameObject.SetActive(false);
            shareCanvas.gameObject.SetActive(false);
        }

        
        public void ActivateDrawing()
        {
            GlobalVariables.activateDraw = true;
            GlobalVariables.activatePanel = false;
            GlobalVariables.drawStop = false;
            controlCanvas.gameObject.SetActive(false);
            drawCanvas.gameObject.SetActive(true);
            selectButton.SetActive(true);
            exitButton.SetActive(true);
            stopDarwButton.SetActive(false);
            deleteButton.SetActive(false);
        }

        public void ActivateSharing()
        {
            GlobalVariables.activateDraw = false;
            GlobalVariables.activatePanel = false;
            GlobalVariables.drawStop = false;
            controlCanvas.gameObject.SetActive(false);
            shareCanvas.gameObject.SetActive(true);
        }

        public void ActivateSelection()
        {
            GlobalVariables.drawSelected = true;
            GlobalVariables.drawStop = false;
            selectButton.SetActive(false);
            exitButton.SetActive(false);
            stopDarwButton.SetActive(true);
            deleteButton.SetActive(true);
        }

        public void ExitDrawing()
        {
            GlobalVariables.activateDraw = false;
            GlobalVariables.activatePanel = true;
            controlCanvas.gameObject.SetActive(true);
            drawCanvas.gameObject.SetActive(false);
            GlobalVariables.drawStop = true;
        }

        public void ExitSharing()
        {
            GlobalVariables.activateDraw = false;
            GlobalVariables.activatePanel = true;
            controlCanvas.gameObject.SetActive(true);
            shareCanvas.gameObject.SetActive(false);
            GlobalVariables.drawStop = true;
        }

        public void StopDrawing()
        {
            GlobalVariables.drawStop = true;
            selectButton.SetActive(true);
            exitButton.SetActive(true);
            stopDarwButton.SetActive(false);
            deleteButton.SetActive(false);
        }

        public void DeleteLastDraw()
        {
            GlobalVariables.drawDelete = true;
        }

    }
}
