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
    public SubjectInfoUI subjectInfoUI;    // Subject info

    [Header("Explanation UI")]
    public GameObject explanationPanel;    // The popup panel for feedback
    public TMP_Text explanationText;       // Text for why it was correct/incorrect
    public Button learnMoreButton;         // Button for external link
    public Button closeExplainBtn;         // Button to finish and move on
    [Tooltip("Explanation body text size (applies after scroll setup).")]
    [SerializeField] float explanationFontSize = 38f;
    [SerializeField] float explanationLineSpacing = 4f;

    private Question currentQuestion;
    public QuestionManager questionManager; // assign in Inspector 

    private ScrollRect _explanationScroll;

    void Start()
    {
        // Find QuestionManager if not assigned
        if(questionManager == null)
            questionManager = Object.FindFirstObjectByType<QuestionManager>();

        EnsureExplanationScrollSetup();
        ApplyExplanationPresentation();

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
        if (subjectInfoUI != null)
            subjectInfoUI.RefreshSubject(question.topic);

        if(questionManager != null)
            questionManager.waitingForAnswer = true;
    }

    void OnAnswerClicked(int choiceIndex)
    {
        if (subjectInfoUI != null && subjectInfoUI.infoPanel != null) 
            subjectInfoUI.infoPanel.SetActive(false);
            
        if(questionManager != null && currentQuestion != null)
        {
            bool isCorrect = (choiceIndex == currentQuestion.answer_index);
            questionManager.lastAnswerCorrect = isCorrect;
            
            // Show the explanation panel before closing the main UI
            ShowExplanation(isCorrect);
        }
    }

    void EnsureExplanationScrollSetup()
    {
        if (explanationText == null || explanationPanel == null) return;

        if (explanationText.GetComponentInParent<ScrollRect>() != null)
        {
            _explanationScroll = explanationText.GetComponentInParent<ScrollRect>();
            ConfigureExplanationScrollRect(_explanationScroll);
            return;
        }

        var panelRt = explanationPanel.GetComponent<RectTransform>();
        var textTransform = explanationText.rectTransform;
        int siblingIndex = textTransform.GetSiblingIndex();

        var scrollGo = new GameObject("ExplanationScroll", typeof(RectTransform));
        scrollGo.transform.SetParent(panelRt, false);
        scrollGo.transform.SetSiblingIndex(siblingIndex);

        var scrollRt = scrollGo.GetComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = new Vector2(18f, 120f);
        scrollRt.offsetMax = new Vector2(-18f, -70f);

        var scrollRect = scrollGo.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        ConfigureExplanationScrollRect(scrollRect);

        var bg = scrollGo.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.01f);
        bg.raycastTarget = true;

        var viewportGo = new GameObject("Viewport", typeof(RectTransform));
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var viewportRt = viewportGo.GetComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.sizeDelta = Vector2.zero;
        viewportRt.anchoredPosition = Vector2.zero;
        viewportGo.AddComponent<RectMask2D>();

        var contentGo = new GameObject("Content", typeof(RectTransform));
        contentGo.transform.SetParent(viewportGo.transform, false);
        var contentRt = contentGo.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.anchoredPosition = Vector2.zero;
        contentRt.sizeDelta = Vector2.zero;

        var vlg = contentGo.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(16, 16, 12, 16);

        var contentFitter = contentGo.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        textTransform.SetParent(contentRt, false);
        textTransform.anchorMin = new Vector2(0f, 1f);
        textTransform.anchorMax = new Vector2(1f, 1f);
        textTransform.pivot = new Vector2(0.5f, 1f);
        textTransform.anchoredPosition = Vector2.zero;
        textTransform.sizeDelta = Vector2.zero;

        var textFitter = explanationText.gameObject.GetComponent<ContentSizeFitter>();
        if (textFitter == null)
            textFitter = explanationText.gameObject.AddComponent<ContentSizeFitter>();
        textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        textFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        var le = explanationText.gameObject.GetComponent<LayoutElement>();
        if (le == null)
            le = explanationText.gameObject.AddComponent<LayoutElement>();
        le.flexibleWidth = 1f;

        explanationText.raycastTarget = false;

        scrollRect.viewport = viewportRt;
        scrollRect.content = contentRt;
        _explanationScroll = scrollRect;
    }

    void ConfigureExplanationScrollRect(ScrollRect scrollRect)
    {
        if (scrollRect == null) return;
        scrollRect.scrollSensitivity = 100f;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
    }

    void ApplyExplanationPresentation()
    {
        if (explanationText == null) return;
        explanationText.fontSize = explanationFontSize;
        explanationText.fontSizeMax = Mathf.Max(explanationText.fontSizeMax, explanationFontSize);
        explanationText.lineSpacing = explanationLineSpacing;
        if (_explanationScroll != null)
            ConfigureExplanationScrollRect(_explanationScroll);
    }

    private string GetSubjectLink(string topic)
    {
        if (string.IsNullOrEmpty(topic)) return "";

        switch (topic)
        {
            case "Math & Logic": 
                return "https://www.khanacademy.org/math";
            case "Environmental Science": 
                return "https://www.ducksters.com/science/environment/";
            case "English Grammar":
                return "https://www.geeksforgeeks.org/english/english-grammar/";
            case "Global Citizenship":
                return "https://www.unesco.org/en/global-citizenship-peace-education";
            
            // If it doesn't match any subject above, return nothing
            default: 
                return ""; 
        }
    }

    void ShowExplanation(bool isCorrect)
    {
        if (explanationPanel == null || explanationText == null) return;

        explanationPanel.SetActive(true);
        string status = isCorrect ? "<color=green>Correct!</color> " : "<color=red>Incorrect.</color> ";
        explanationText.text = status + currentQuestion.explanation;

        Canvas.ForceUpdateCanvases();
        if (_explanationScroll != null && _explanationScroll.content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_explanationScroll.content);
            _explanationScroll.verticalNormalizedPosition = 1f;
        }

        if (learnMoreButton != null)
        {
            string categoryUrl = GetSubjectLink(currentQuestion.topic);
            
            bool hasUrl = !string.IsNullOrEmpty(categoryUrl);
            learnMoreButton.gameObject.SetActive(hasUrl);
            
            if (hasUrl)
            {
                learnMoreButton.onClick.RemoveAllListeners();
                learnMoreButton.onClick.AddListener(() => Application.OpenURL(categoryUrl));
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