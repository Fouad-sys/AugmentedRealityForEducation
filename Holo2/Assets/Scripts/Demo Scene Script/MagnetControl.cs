
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
namespace Com.MyCompany.MyGame
{
    public class MagnetControl : MonoBehaviourPun
    {
        private bool isSelected1 = false;
        private bool isSelected2 = false;
        private bool isSelected3 = false;
        private Color objectColor;

        private LineRenderer lineRenderer;
        public Material lineMaterial;
        public GameObject linePrefab;
        public Transform clickedTransform;


        public List<LineRenderer> lineRendererList;
        public List<LineRenderer> remoteLineRendererList;
        private bool isDragging = false;
        private bool firstclick = true;
        private float CameraZDistance;
        private bool isDrawing = false;
        public bool isSelected = false;
        private Vector3 lastRecordedPoint;
        private Color currentColor;
        private Renderer renderer;
        private int previousID = 0;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Vector3 lastScale;


        private void Start()
        {
            Debug.Log($"Is MasterClient: {PhotonNetwork.IsMasterClient}");
            lineRendererList = new List<LineRenderer>();
            Debug.Log("Create lineRendererList here");
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            lastScale = transform.localScale;
        }

        private void PanelLogic(ref QuestionAnswerSystem qAsys, ref bool isSelected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                isDragging = true;
                if (!isSelected)
                {
                    if (GlobalVariables.onlyOne)
                    {
                        qAsys.ShowTeacherPanel();
                        isSelected = !isSelected;
                        GlobalVariables.onlyOne = false;
                    }
                }
                else
                {
                    qAsys.CloseTeacherPanel();
                    isSelected = !isSelected;
                    GlobalVariables.onlyOne = true;
                }
            }
            else
            {
                isSelected = !isSelected;
                isDragging = false;
                if (!isSelected)
                {
                    if (GlobalVariables.onlyOne)
                    {
                        qAsys.ShowPanel();
                        GlobalVariables.onlyOne = false;
                    }
                }
                else
                {
                    qAsys.ClosePanel();
                    GlobalVariables.onlyOne = true;
                }
            }
        }

        private void OnMouseUp()
        {
            Debug.Log("Release Mouse here");
            isDragging = false;
            firstclick = true;
        }

        private void Update()
        {
            if (!transform.position.Equals(lastPosition) || !transform.rotation.Equals(lastRotation) || !transform.localScale.Equals(lastScale))
            {
                SendPosition(transform.position, transform.rotation, transform.localScale);
                lastPosition = transform.position;
                lastRotation = transform.rotation;
                lastScale = transform.localScale;
            }
            if (GlobalVariables.shareActivate)
            {
                Debug.Log($"share {GlobalVariables.shareID} work here");
                ShowLineRenderer(GlobalVariables.shareID);
            }
            else if (GlobalVariables.shareDeActivate)
            {
                Debug.Log($"share {GlobalVariables.shareID} deactivate here");
                DeactivateShowLineRenderer(GlobalVariables.shareID);
                GlobalVariables.shareDeActivate = false;
            }
            // Object Drawing
            if (GlobalVariables.activateDraw)
            {

                if (Input.GetMouseButtonDown(0) & GlobalVariables.drawSelected)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        MagnetControl magnetControl = hit.collider.gameObject.GetComponent<MagnetControl>();
                        if (magnetControl != null)
                        {
                            renderer = hit.collider.gameObject.GetComponent<Renderer>();
                            if (renderer != null)
                            {
                                Material material = renderer.material;

                                currentColor = material.color;
                                Color newColor = Color.yellow;

                                renderer.material.color = newColor;
                            }
                            else
                            {
                                Debug.Log("Renderer not found on clicked object.");
                            }
                            Debug.Log("Magnet Hit here.");
                            GlobalVariables.drawSelected = false;

                        
                                hit.collider.gameObject.GetComponent<MagnetControl>().isSelected = true;
                        
                        
                            clickedTransform = hit.transform;

                            GlobalVariables.drawStop = false;
                        }
                    }
                }
                if (isSelected)
                {
                    if (Input.GetMouseButtonDown(0) & !isDrawing)
                    {
                        GameObject lineRendererBlock = Instantiate(linePrefab);
                        lineRendererBlock.transform.SetParent(clickedTransform);

                        lineRenderer = lineRendererBlock.GetComponent<LineRenderer>();
                        StartDrawing();
                    }

                    if (isDrawing)
                    {

                        Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(clickedTransform.position).z);
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(ScreenPosition);

                        if (Input.GetMouseButton(0))
                        {
                            if (mousePos != lastRecordedPoint)
                            {
                                AddPointToLine(mousePos);
                                lastRecordedPoint = mousePos;
                            }
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            StopDrawing();
                        }
                    }
                    if (GlobalVariables.drawDelete & lineRendererList.Count > 0)
                    {
                        LineRenderer lastLineRenderer = lineRendererList[lineRendererList.Count - 1];
                        lineRendererList.Remove(lastLineRenderer);
                        Destroy(lastLineRenderer.gameObject); // destroy LineRenderer's parent gameobject
                        photonView.RPC("DeleteLastLineRenderer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
                        GlobalVariables.drawDelete = false;
                        Debug.Log("lineRendererList.Count" + lineRendererList.Count);
                    }
                }
                if (GlobalVariables.drawStop)
                {
                    isSelected = false;
                    if (renderer != null)
                    {
                        renderer.material.color = currentColor;
                    }

                }

            }
            else if (GlobalVariables.shareActivate)
            {
                Debug.Log($"share {GlobalVariables.shareID} work here");
                ShowLineRenderer(GlobalVariables.shareID);
            }
            else if (GlobalVariables.shareDeActivate)
            {
                Debug.Log($"share {GlobalVariables.shareID} deactivate here");
                DeactivateShowLineRenderer(GlobalVariables.shareID);
                GlobalVariables.shareDeActivate = false;
            }
            else
            {
                // Object Movement
                if (isDragging)
                {
                    Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraZDistance);

                    if (Input.GetMouseButton(0))
                    {
                        if (firstclick)
                        {
                            lastRecordedPoint = ScreenPosition;
                            firstclick = false;
                        }
                        if (ScreenPosition != lastRecordedPoint && !firstclick)
                        {
                            Vector3 NewWlorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
                            Vector3 LastWlorldPosition = Camera.main.ScreenToWorldPoint(lastRecordedPoint);
                            transform.position += NewWlorldPosition- LastWlorldPosition;
                            lastRecordedPoint = ScreenPosition;
                        }
                    }
                }

            }
        }

        void AddPointToLine(Vector3 point)
        {
            int pointCount = lineRenderer.positionCount;
            lineRenderer.positionCount = pointCount + 1;
            lineRenderer.SetPosition(pointCount, point);
        }

        void StartDrawing()
        {
            isDrawing = true;
            lineRenderer.positionCount = 0;
        }

        void StopDrawing()
        {
            isDrawing = false;
            if (lineRenderer.positionCount > 3)
            {
                lineRendererList.Add(lineRenderer);
                SendLineRenderersData();
                Debug.Log("Draw lineRendererList.Count" + lineRendererList.Count);
            }
            else
            {
                Destroy(lineRenderer.gameObject);
            }
        }


        [PunRPC]
        private void SyncPosition(float postionX, float postionY, float postionZ,
            float rotationX, float rotationY, float rotationZ, float rotationW,
            float scaleX, float scaleY, float scaleZ)
        {
            transform.position = new Vector3(postionX, postionY, postionZ);
            transform.rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }

        public void SendPosition(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            photonView.RPC("SyncPosition", RpcTarget.AllBuffered, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, scale.x, scale.y, scale.z);
        }

        void SendLineRenderersData()
        {
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            photonView.RPC("UpdateLineRenderer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, positions);
            
        }

        [PunRPC]
        void UpdateLineRenderer(int id, Vector3[] positions)
        {
            Debug.Log("Id in update linerender is: " + id);
            Debug.Log("Positions in update linerender are: " + positions);
            GameObject lineRendererBlock = Instantiate(linePrefab);
            lineRendererBlock.transform.SetParent(clickedTransform);
            LineRenderer updatelineRenderer = lineRendererBlock.GetComponent<LineRenderer>();
            updatelineRenderer.enabled = false;

            updatelineRenderer.positionCount = positions.Length;
            updatelineRenderer.SetPositions(positions);
            
            if (PhotonNetwork.IsMasterClient)
            {
                if (!GlobalVariables.studentRemoteLineRendererList.ContainsKey(id))
                {
                    Debug.Log("In Update Line render IMasterClient " + id);
                    GlobalVariables.studentRemoteLineRendererList[id] = new List<LineRenderer>();
                }
                GlobalVariables.studentRemoteLineRendererList[id].Add(updatelineRenderer);
            }
        }

        void ShowLineRenderer(int id)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (LineRenderer iterLineRenderer in lineRendererList)
                {
                    if (iterLineRenderer != null)
                    {
                        iterLineRenderer.enabled = false;
                    }
                }
                if (previousID != 0)
                {
                    foreach (LineRenderer existingLineRenderer in GlobalVariables.studentRemoteLineRendererList[previousID])
                    {
                        if (existingLineRenderer != null)
                        {
                            existingLineRenderer.enabled = false;
                        }
                    }

                }
                foreach (LineRenderer existingLineRenderer in GlobalVariables.studentRemoteLineRendererList[id])
                {
                    existingLineRenderer.enabled = true;
                }

                previousID = id;
            }
        }

        void CopyLineRendererProperties(LineRenderer source, LineRenderer destination)
        {
            if (source != null && destination != null)
            {
                destination.startWidth = source.startWidth;
                destination.endWidth = source.endWidth;
                destination.material = source.material;
                Debug.Log($"Copy here, and soure position count is {source.positionCount}");
                // Assuming you use SetPositions and GetPositions for copying line positions
                Vector3[] positions = new Vector3[source.positionCount];
                source.GetPositions(positions);
                destination.SetPositions(positions);
            }
        }


        void DeactivateShowLineRenderer(int id)
        {
            foreach (LineRenderer iterLineRenderer in GlobalVariables.studentRemoteLineRendererList[id])
            {
                if (iterLineRenderer != null)
                {
                    iterLineRenderer.enabled = false;
                }
            }

            foreach (LineRenderer iterLineRenderer in lineRendererList)
            {
                if (iterLineRenderer != null)
                {
                    iterLineRenderer.enabled = true;
                }
            }
        }

        [PunRPC]
        void DeleteLastLineRenderer(int id)
        {
            if (GlobalVariables.studentRemoteLineRendererList[id].Count > 0)
            {
                
                LineRenderer lastLineRenderer = GlobalVariables.studentRemoteLineRendererList[id][GlobalVariables.studentRemoteLineRendererList[id].Count - 1];

                if (PhotonNetwork.IsMasterClient)
                {
                    GlobalVariables.studentRemoteLineRendererList[id].Remove(lastLineRenderer);
                    Destroy(lastLineRenderer.gameObject);
                }
            }
        }
    }
}