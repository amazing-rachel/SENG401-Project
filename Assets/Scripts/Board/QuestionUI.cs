using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;               // The QuestionPanel GameObject
    public TMP_Text questionText;          // TextMeshPro for question
    public List<Button> answerButtons;     // 4 answer buttons

    private Question currentQuestion;
    public QuestionManager questionManager; // assign in Inspector 

    void Start()
    {
        // Find QuestionManager if not assigned
        if(questionManager == null)
            questionManager = Object.FindFirstObjectByType<QuestionManager>();

        if(panel != null)
            panel.SetActive(false); // hide panel at start

        // Set up button listeners
        for (int i = 0; i < answerButtons.Count; i++)
        {
            int index = i; // local copy 
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
        }
    }

    public void ShowQuestion(Question question)
    {
        if (panel == null || questionText == null || answerButtons == null)
        {
            Debug.LogError("QuestionUI references not assigned!");
            return;
        }

        currentQuestion = question;
        panel.SetActive(true);

        // Show question text
        questionText.text = question.question;

        // button texts using TMP_Text
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if(i >= question.choices.Count)
            {
                answerButtons[i].gameObject.SetActive(false); // hide extra buttons
                continue;
            }

            answerButtons[i].gameObject.SetActive(true); // make visible
            TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            if(btnText != null)
                btnText.text = question.choices[i];
            else
                Debug.LogError("Button " + i + " is missing TMP_Text component!");
        }

        if(questionManager != null)
            questionManager.waitingForAnswer = true;
    }

    void OnAnswerClicked(int choiceIndex)
    {
        if(questionManager != null && currentQuestion != null)
        {
            questionManager.lastAnswerCorrect = (choiceIndex == currentQuestion.answer_index);
            questionManager.waitingForAnswer = false;
        }

        if(panel != null)
            panel.SetActive(false);
    }
}