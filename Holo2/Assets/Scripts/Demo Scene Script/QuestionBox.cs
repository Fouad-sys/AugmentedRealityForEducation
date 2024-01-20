using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionBox : MonoBehaviour
{
    public int questionId;
    public Color color;
    public string question;
    public Vector3 position;
    private GameObject cube;
    private GameObject go;
    private TextMeshPro text;

    public QuestionBox(int questionId, Color color, string question, Vector3 position)
    {
        this.questionId = questionId;
        this.color = color;
        this.question = question;
        this.position = position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayQuestionBox()
    {
        cube.SetActive(true);
        text.alpha = 1.0f;
        
    }

    public void createQuestionBox()
    {

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.rotation = Quaternion.identity;
        cube.transform.localScale = new Vector3(1f, 0.5f, 0.5f);
        cube.GetComponent<Renderer>().material.color = color;
        cube.SetActive(false);

        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(position.x + 80, position.y, position.z + 4);
        text = go.AddComponent<TextMeshPro>();
        text.text = question;
        text.transform.position = cube.transform.forward * (-0.5f);
        //text.transform.position = new Vector3(position.x + 10.25f, position.y - 2.25f, position.z - 1);
        text.fontSize = 0.5f;
        text.alpha = 0f;
    }
}
