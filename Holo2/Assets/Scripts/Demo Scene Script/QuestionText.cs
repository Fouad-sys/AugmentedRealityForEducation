using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionText : MonoBehaviour
{

    public QuestionText(string question)
    {
        this.GetComponent<TextMeshProUGUI>().text = question;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
