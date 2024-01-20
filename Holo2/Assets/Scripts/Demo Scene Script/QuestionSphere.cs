using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestionSphere;
using Photon.Pun;
using Photon.Voice.PUN.UtilityScripts;
using Photon.Pun.UtilityScripts;
using Com.MyCompany.MyGame;

public class QuestionSphere : MonoBehaviourPun
{

    public struct Question
    {
        public string question;
        public int index;
        public Color color;
    }

    private int questionUniqueIndex = 0;

    public struct QuestionItem
    {
        public string question;
        public int index;
        public int votes;
        public Color color;
    }
    private List<QuestionItem> questionItems = new List<QuestionItem>();

    public List<Question> questions = new List<Question>();
    public List<GameObject> questionBoxes = new List<GameObject>();

    public Vector3 startingPosition;

    public GameObject questionBox;
    public GameObject questionPanel;
    public GameObject QASystem;
    private bool clicked = false;
    private int questionsSize = 0;
    private int oldQuestionsSize = 0;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = this.transform.position;
        questionPanel.SetActive(false);
        QASystem.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        questionsSize = QASystem.GetComponent<QuestionAnswerSystem>().getQuestionItems().Count;
        if (questionsSize > oldQuestionsSize)
        {
            this.transform.localScale += new Vector3(0.4f * questionsSize, 0.4f * questionsSize, 0.4f * questionsSize);
            oldQuestionsSize = questionsSize;
        } else if (questionsSize < oldQuestionsSize)
        {
            this.transform.localScale -= new Vector3(0.4f * questionsSize, 0.4f * questionsSize, 0.4f * questionsSize);
            oldQuestionsSize = questionsSize;
        }
    }

    /*
    private void OnMouseOver()
    {
       
        Cubes[] cubes = FindObjectsOfType(typeof(Cubes)) as Cubes[];
        Texts[] texts = FindObjectsOfType(typeof(Texts)) as Texts[];
        print(cubes.Length);
        print(texts.Length);
        
        foreach (QuestionBox qb in questionBoxes)
        {
            qb.displayQuestionBox();
        }
        
        foreach(Cubes c in cubes)
        {
            Vector3 pos = c.gameObject.transform.position;
            c.gameObject.transform.position = new Vector3(startingPosition.x-2, pos.y, pos.z);
        }
        foreach(Texts text in texts)
        {
            Vector3 pos = text.gameObject.transform.position;
            text.gameObject.transform.position = new Vector3(startingPosition.x-2.15f, pos.y, pos.z);
        }
        
        
        GameObject newQuestionBox = Instantiate(questionBox, new Vector3(startingPosition.x - 2, startingPosition.y -2*questions.Count+2, startingPosition.z), Quaternion.identity);
        newQuestionBox.GetComponent<TextMeshPro>().text = "hey";
        
        
        Debug.Log("Hovered over");
        foreach (GameObject g in questionBoxes)
        {
            g.SetActive(true);
        }
        print(questions.Count);
        questionPanel.SetActive(true);
        QASystem.SetActive(true);

    }
    */

    public void receiveQuestion(Question item)
    {
        questions.Add(item);
        this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        GameObject newQuestionBox = Instantiate(questionBox, new Vector3(startingPosition.x - 2, startingPosition.y - 1 * questions.Count + 2, startingPosition.z), Quaternion.identity);
        Canvas c = newQuestionBox.GetComponentInChildren<Canvas>();
        print(c);
        TextMeshPro text = newQuestionBox.GetComponentInChildren<TextMeshPro>();
        questionBoxes.Add(newQuestionBox);
    }

    public void displayQuestions()
    {
        clicked = !clicked;
        Debug.Log("Sphere Clicked");
        if (clicked)
        {
            questionPanel.SetActive(true);
            QASystem.SetActive(true);
        } else
        {
            questionPanel.SetActive(false);
            QASystem.SetActive(false);
        }
    }
}
