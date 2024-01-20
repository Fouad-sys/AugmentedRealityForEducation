using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;

namespace Com.MyCompany.MyGame
{
    public class StudentControlManager : MonoBehaviourPunCallbacks
    {
        public GameObject buttonPrefab; // Assign this in the Unity Inspector
        public Transform buttonParent;  // Assign a parent object in Inspector for layout
        public Transform parentTransform;

        private float verticalSpacing = 95f; // vertical spacing

        private List<int> studentIds = new List<int>();
        private Dictionary<int, bool> drawingPermissions = new Dictionary<int, bool>();
        private Dictionary<int, GameObject> drawingButtons = new Dictionary<int, GameObject>();

        private void Start()
        {
            /*
            GlobalVariables.studentIDName[223] = "Fouad";
            GlobalVariables.studentIDName[224] = "Beth";
            GlobalVariables.studentIDName[225] = "Sarah";
            RefreshPlayerButton();
            */
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient) { 
                int studentId = newPlayer.ActorNumber;
                string stuNickname = newPlayer.NickName;

                Debug.Log($"ID: {studentId}, Nickname: {stuNickname}");
                GlobalVariables.studentIDName[studentId] = stuNickname;
                RefreshPlayerButton();
            }

        }

        void CreatePlayerButton(int studentId, string nickname, int posindex)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, parentTransform);
            TMP_Text playerText = buttonObj.GetComponentInChildren<TMP_Text>();
            playerText.text = nickname;
            Button button = buttonObj.GetComponent<Button>();
            Vector2 buttonPosition = button.GetComponent<RectTransform>().anchoredPosition;
            buttonPosition.y -= verticalSpacing * posindex;
            button.GetComponent<RectTransform>().anchoredPosition = buttonPosition;
            button.GetComponent<Image>().color = new Color(0f, 0.051f, 1f);
            posindex += 1;
            button.onClick.AddListener(() => ShareActivation(studentId));
            if (!drawingPermissions.ContainsKey(studentId))
            {
                studentIds.Add(studentId);
                drawingPermissions.Add(studentId, false);
                drawingButtons.Add(studentId, buttonObj);
            } else if (drawingPermissions[studentId] == true)
            {
                button.GetComponent<Image>().color = new Color(0f, 0.035f, 0.651f);
                drawingButtons.Add(studentId, buttonObj);
            }
        }

        void RefreshPlayerButton()
        {
            int posindex = 0;

            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
            foreach (KeyValuePair<int, string> kvp in GlobalVariables.studentIDName)
            {
                CreatePlayerButton(kvp.Key, kvp.Value, posindex);
                posindex = posindex + 1;
            }
        }

        public override void OnPlayerLeftRoom(Player exitPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (GlobalVariables.studentIDName.ContainsKey(exitPlayer.ActorNumber))
                {
                    GlobalVariables.studentIDName.Remove(exitPlayer.ActorNumber);
                    RefreshPlayerButton();
                }
            }
        }

        void ShareActivation(int id)
        {
            Debug.Log("Clicked on Drawing Button");
            if (drawingPermissions[id] == false)
            {
                Debug.Log("FALSEE");
                GlobalVariables.shareID = id;
                GlobalVariables.shareActivate = true;
                foreach (int studentId in GlobalVariables.studentIDName.Keys)
                {
                    if (studentId == id)
                    {
                        Debug.Log("enabledddd");
                        drawingPermissions[studentId] = true;
                        drawingButtons[studentId].GetComponent<Button>().GetComponent<Image>().color = new Color(0f, 0.035f, 0.651f);
                    }
                    else
                    {
                        drawingPermissions[studentId] = false;
                        drawingButtons[studentId].GetComponent<Button>().GetComponent<Image>().color = new Color(0f, 0.051f, 1f);
                    }
                }
            } else
            {
                drawingPermissions[id] = false;
                drawingButtons[id].GetComponent<Button>().GetComponent<Image>().color = new Color(0f, 0.051f, 1f);
                GlobalVariables.shareActivate = false;
                GlobalVariables.shareDeActivate = true;
            }
        }

        public void ShareDeActivation()
        {
            GlobalVariables.shareActivate = false;
            GlobalVariables.shareDeActivate = true;
        }

        public void DisplayAllStudentInfo()
        {
            foreach (KeyValuePair<int, List<LineRenderer>> entry in GlobalVariables.studentRemoteLineRendererList)
            {
                Debug.Log($"ID: {entry.Key} LineRenderer List len: {entry.Value.Count}");
            }
        }

        private string PositionArrayToString(Vector3[] positions)
        {
            if (positions == null || positions.Length == 0)
                return "[]";

            string positionsString = "[";
            foreach (Vector3 pos in positions)
            {
                positionsString += pos.ToString() + ", ";
            }
            positionsString = positionsString.TrimEnd(',', ' ') + "]";
            return positionsString;
        }

        public void AddOrUpdateStudentControl(int id, string nickname, bool grantSharing)
        {
            StudentControl control = new StudentControl(id, nickname, grantSharing);
            GlobalVariables.studentControls[id] = control;
        }

        void CreateButtonsForLineRenderers()
        {
            foreach (var entry in GlobalVariables.studentRemoteLineRendererList)
            {
                int studentId = entry.Key;
                GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
                buttonObj.GetComponentInChildren<Text>().text = "Student" + studentId+": "+GlobalVariables.studentIDName[studentId];
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => DisplayLineRenderers(studentId));
            }
        }

        void DisplayLineRenderers(int studentId)
        {
            if (GlobalVariables.studentRemoteLineRendererList.ContainsKey(studentId))
            {
                foreach (var lineRenderer in GlobalVariables.studentRemoteLineRendererList[studentId])
                {
                    // Code to display or activate the LineRenderer
                    lineRenderer.enabled = true; // Example: Enable the LineRenderer
                }
            }
        }

        [PunRPC]
        public void UpdateStudentControl(int id, string nickname, bool grantSharing, Vector3[] positions)
        {
            AddOrUpdateStudentControl(id, nickname, grantSharing);
            DisplayAllStudentInfo();
        }

        public void BroadcastStudentControlUpdate(int id, string nickname, bool grantSharing)
        {
            photonView.RPC("UpdateStudentControl", RpcTarget.All, id, nickname, grantSharing);
        }
    }
}