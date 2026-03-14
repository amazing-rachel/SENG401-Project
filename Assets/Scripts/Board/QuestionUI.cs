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
    public Button hintButton;              // Button to trigger hint
    public TMP_Text hintText;              // Text field to display hint

    [Header("Explanation UI")]
    public GameObject explanationPanel;    // The popup panel for feedback
    public TMP_Text explanationText;       // Text for why it was correct/incorrect
    public Button learnMoreButton;         // Button for external link
    public Button closeExplainBtn;          // Button to finish and move on

    private Question currentQuestion;
    public QuestionManager questionManager; // assign in Inspector 

    void Start()
    {
        // Find QuestionManager if not assigned
        if(questionManager == null)
            questionManager = Object.FindFirstObjectByType<QuestionManager>();

        if(panel != null)
            panel.SetActive(false); // hide panel at start

        // Set up explanation panel button
        if (closeExplainBtn != null)
            closeExplainBtn.onClick.AddListener(CloseExplanation);

        // Set up button listeners
        for (int i = 0; i < answerButtons.Count; i++)
        {
            int index = i; // local copy 
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
        }

        // Set up hint button listener
        if (hintButton != null)
            hintButton.onClick.AddListener(OnHintClicked);
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
        if (explanationPanel != null) explanationPanel.SetActive(false);

        // Show question text
        questionText.text = question.question;

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if(i >= question.choices.Count)
            {
                answerButtons[i].gameObject.SetActive(false); 
                continue;
            }

            answerButtons[i].gameObject.SetActive(true); 
            TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            if(btnText != null)
                btnText.text = question.choices[i];
            else
                Debug.LogError("Button " + i + " is missing TMP_Text component!");
        }

        // Reset hint UI for new question
        if (hintText != null) { hintText.text = ""; hintText.gameObject.SetActive(false); }
        if (hintButton != null) { hintButton.gameObject.SetActive(true); hintButton.interactable = true; }

        if(questionManager != null)
            questionManager.waitingForAnswer = true;
    }

    void OnHintClicked()
    {
        if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.hint))
        {
            if (hintText != null)
            {
                hintText.text = "Hint: " + currentQuestion.hint;
                hintText.gameObject.SetActive(true);
            }
            if (hintButton != null) hintButton.interactable = false;
        }
    }

    void OnAnswerClicked(int choiceIndex)
    {
        if(questionManager != null && currentQuestion != null)
        {
            bool isCorrect = (choiceIndex == currentQuestion.answer_index);
            questionManager.lastAnswerCorrect = isCorrect;
            
            // Show the explanation panel before closing the main UI
            ShowExplanation(isCorrect);
        }
    }

    void ShowExplanation(bool isCorrect)
    {
        if (explanationPanel == null) return;

        explanationPanel.SetActive(true);
        string status = isCorrect ? "<color=green>Correct!</color> " : "<color=red>Incorrect.</color> ";
        explanationText.text = status + currentQuestion.explanation;

        // Learn More logic
        if (learnMoreButton != null)
        {
            bool hasUrl = !string.IsNullOrEmpty(currentQuestion.learnMoreUrl);
            learnMoreButton.gameObject.SetActive(hasUrl);
            if (hasUrl)
            {
                learnMoreButton.onClick.RemoveAllListeners();
                learnMoreButton.onClick.AddListener(() => Application.OpenURL(currentQuestion.learnMoreUrl));
            }
        }
    }

    void CloseExplanation()
    {
        if (explanationPanel != null) explanationPanel.SetActive(false);
        if (panel != null) panel.SetActive(false);
        if (questionManager != null)
            questionManager.waitingForAnswer = false;
    }
}