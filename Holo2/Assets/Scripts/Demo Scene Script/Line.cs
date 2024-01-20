using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class Line : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public Material lineMaterial;

        private Vector3 LastParentTransfom;
        Transform parentTransform;
        List<Vector3> relativePointList = new List<Vector3>();


        // Start is called before the first frame update
        void Start()
        {
            parentTransform = transform.parent;
            Debug.Log("start line parentTransform.position: " + parentTransform.position);

            lineRenderer.positionCount = 0;
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {

            if (GlobalVariables.activateDraw && Input.GetMouseButtonUp(0))
            {
                CalculateRelativePositions(parentTransform);
            }
            if (!GlobalVariables.activateDraw && parentTransform.position != LastParentTransfom)
        {
                UpdateLineRenderer();
                LastParentTransfom = parentTransform.position;
            }

        }

        void CalculateRelativePositions(Transform initialParentTransform)
        {
            //Transform initialTransform = transform.parent;
            Debug.Log("initialParentTransform Point Position: " + initialParentTransform.position);

            if (initialParentTransform != null && lineRenderer != null)
            {
                int pointCount = transform.GetComponent<LineRenderer>().positionCount;
                Debug.Log("Point Count: " + pointCount);

                for (int i = 0; i < pointCount; i++)
                {
                    // get wordposition of the points in LineRenderer
                    Vector3 worldPoint = lineRenderer.GetPosition(i);
                    Debug.Log("Point Position: " + worldPoint);

                    // relative position
                    Vector3 relativePosition = initialParentTransform.InverseTransformPoint(worldPoint);

                    relativePointList.Add(relativePosition);
                }

            }
            else
            {
                Debug.LogWarning("Parent transform or LineRenderer not set.");
            }
        }

        void UpdateLineRenderer()
        {
            if (lineRenderer != null && relativePointList.Count == lineRenderer.positionCount)
            {
                Debug.Log("update Relative Count: " + relativePointList.Count);

                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    Vector3 worldPosition = parentTransform.TransformPoint(relativePointList[i]);

                    lineRenderer.SetPosition(i, worldPosition);
                }
            }
        }
    }
}