using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using System.Text;

namespace Com.MyCompany.MyGame
{
    public class QuestionAnswerSystem : MonoBehaviourPun
    {
        public Camera mainCamera;
        public Text titleText;
        public Text contentText; // dislay qustion and answers
        public Canvas questionPanel;
        public Transform parentTransform;
        public GameObject textBlockPrefab;
        public GameObject deleteButtonPrefab;
        private int questionUniqueIndex = 0;
        private List<int> isVoted;
        private float verticalButtonSpacing = 30f; // vertical spacing
        private float currentYPosition = -10f;
        private float verticalSpacing = 15f;
        private float extraSpacing = 5f;


        private int panelLayer;

        public struct QuestionItem
        {
            public string question;
            public int index;
            public int votes;
            public Color color;
        }
        private List<QuestionItem> questionItems = new List<QuestionItem>();

        private List<string> questions = new List<string>();

        private void Start()
        {
            questions = new List<string>();
        }

        private void Update()
        {
            if (mainCamera != null)
            {
                questionPanel.transform.LookAt(mainCamera.transform);

                // caculate the facing direction 
                Vector3 lookDirection = mainCamera.transform.position - questionPanel.transform.position;
                // make canvas face the camera
                questionPanel.transform.rotation = Quaternion.LookRotation(-lookDirection);
            }
        }

        public List<QuestionItem> getQuestionItems()
        {
            return questionItems;
        }

        public void ShowPanel()
        {
            questionPanel.gameObject.SetActive(true);
        }

        public void ShowTeacherPanel()
        {
            questionPanel.gameObject.SetActive(true);
        }

        public void CloseTeacherPanel()
        {
            questionPanel.gameObject.SetActive(false);
        }

        public void ClosePanel()
        {
            panelLayer = 0;
            questionPanel.gameObject.SetActive(false);
        }

        public void AddQuestionItem()
        {

            Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
            QuestionItem item = new QuestionItem
            {
                index = questionUniqueIndex, //questionItems.Count,  set the index fot question(fixed)
                votes = 1,
                color = randomColor //ramdon color fixed
            };
            questionUniqueIndex += 1;
            questionItems.Add(item);
            RefreshQuestionItems();
        }

        private void RefreshQuestionItems()
        {
            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
            int posindex = 0;
            currentYPosition = -10;
            questionItems = questionItems.OrderByDescending(item => item.votes).ToList();
            foreach (QuestionItem questionItem in questionItems)
            {
                CreateQuestionItem(questionItem, posindex);
                posindex = posindex + 1;
            }
        }

        string InsertNewLine(string text, int charLimit, out int lineCount)
        {
            StringBuilder result = new StringBuilder();
            lineCount = 0;

            while (text.Length > 0)
            {
                int length = System.Math.Min(charLimit, text.Length);
                string line = text.Substring(0, length);
                result.AppendLine(line); // AppendLine automatically adds a newline character
                text = text.Substring(length);
                lineCount++;
            }

            return result.ToString().TrimEnd();
        }


        private void CreateQuestionItem(QuestionItem questionItem, int posindex)
        {
            GameObject textBlock = Instantiate(textBlockPrefab, parentTransform);
            Text textComponent = textBlock.GetComponentInChildren<Text>();
            Image textImg = textBlock.GetComponent<Image>();
            string text = "Q" + questionItem.index + "(" + questionItem.votes + "): " + questionItem.question;
            int lineCount;
            textComponent.text = InsertNewLine(text, 30, out lineCount);
            // set text position
            RectTransform textBlockTransform = textBlock.GetComponent<RectTransform>();
            float lineHeight = textComponent.fontSize * textComponent.lineSpacing;
            float totalTextHeight = lineHeight * lineCount;
            float blockHeight = totalTextHeight + extraSpacing;
            currentYPosition -= blockHeight;
            Vector2 position = new Vector2(0, currentYPosition);
            textBlockTransform.anchoredPosition = position;
            float textWidth = textComponent.preferredWidth;
            float maxWidth = Mathf.Max(textWidth, 200f);
            textBlockTransform.sizeDelta = new Vector2(maxWidth, totalTextHeight);
            currentYPosition -= verticalSpacing; 
            // set text backgroud
            textImg.color = questionItem.color;
            // add listner for delete and upvote item
            if (PhotonNetwork.IsMasterClient)
            {
                // new delete button
                // Create and set upvote button
                GameObject deleteButton = Instantiate(deleteButtonPrefab, parentTransform);
                Button deleteButtonComponent = deleteButton.GetComponent<Button>();
                RectTransform deleteButtonTransform = deleteButton.GetComponent<RectTransform>();
                //set the position delete button
                Vector2 deleteButtonPosition = deleteButtonTransform.anchoredPosition;
                deleteButtonPosition.y = 0 - verticalButtonSpacing * posindex;
                deleteButtonPosition.y = position.y + 20;
                deleteButtonTransform.anchoredPosition = deleteButtonPosition;
                deleteButtonComponent.onClick.AddListener(() => DeleteQuestionItem(questionItem.index));
            }
        }

        public void DeleteQuestionItem(int index)
        {
            QuestionItem itemToRemove = questionItems.Find(item => item.index == index);
            if (itemToRemove.question != null)
            {
                questionItems.Remove(itemToRemove);
                SendQuestionItemsToOtherHost();
                RefreshQuestionItems();
            }
        }

        public void UpvoteQuestionItem(int index)
        {
            if (!isVoted.Contains(index))
            {
                int itemIndex = questionItems.FindIndex(item => item.index == index);
                if (itemIndex != -1)
                {
                    QuestionItem itemUpvote = questionItems[itemIndex];
                    itemUpvote.votes += 1;
                    questionItems[itemIndex] = itemUpvote;
                    isVoted.Add(index);
                    RefreshQuestionItems();
                }
            }
        }

        [PunRPC]
        private void ReceiveQuestionItems(string[] receivedQuestions, float[] colorindexs, int UniqueIndex)
        {
            questionUniqueIndex = UniqueIndex;
            questionItems = new List<QuestionItem>();
            for (int i = 0; i < colorindexs.Length; i += 6)
            {
                int Qindex = (int)colorindexs[i];
                Color randomColor = new Color(colorindexs[i + 1], colorindexs[i + 2], colorindexs[i + 3], colorindexs[i + 4]);
                int Votes = (int)colorindexs[i + 5];
                QuestionItem item = new QuestionItem
                {
                    question = receivedQuestions[i / 6],
                    index = Qindex,
                    color = randomColor,
                    votes = Votes
                };
                questionItems.Add(item);
            }
            RefreshQuestionItems();
        }

        public void SendQuestionItemsToOtherHost()
        {
            // transform questionItems to arrays and send them to other players
            List<string> questionItemstring = new List<string>();
            List<float> colorindexList = new List<float>();
            foreach (QuestionItem questionItem in questionItems)
            {
                questionItemstring.Add(questionItem.question);
                float[] floatArray = { questionItem.index, questionItem.color.r, questionItem.color.g, questionItem.color.b, questionItem.color.a, questionItem.votes };
                colorindexList.AddRange(floatArray);
            }
            photonView.RPC("ReceiveQuestionItems", RpcTarget.Others, (object)questionItemstring.ToArray(), (object)colorindexList.ToArray(), (object)questionUniqueIndex);
        }
    }
}
