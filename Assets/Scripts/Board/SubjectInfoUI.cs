using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubjectInfoUI : MonoBehaviour
{
    public GameObject infoPanel; // Info Panel Window
    public TMP_Text infoText;    // Info Text Component
    public Button infoButton;    // Button to Open Info Panel
    public Button closeInfoBtn;  // Button to Close Info Panel

    private QuestionManager qm;  // Refers to QuestionManager

    void Awake()
    {
        // Accesses subject info dictionary
        qm = Object.FindFirstObjectByType<QuestionManager>();

        if (infoPanel != null) 
            infoPanel.SetActive(false);
        
        // Click listener to the info button to show the info panel
        if (infoButton != null) 
            infoButton.onClick.AddListener(() => infoPanel.SetActive(true));
        
        // Click listener to the info button to show the info panel
        if (closeInfoBtn != null) 
            closeInfoBtn.onClick.AddListener(() => infoPanel.SetActive(false));
    }

    // Updates the subject info based on the selected subject
    public void RefreshSubject(string subject)
    {
        string key = subject.Trim().ToLower();
        if (qm != null && qm.subjectInfoData.TryGetValue(key, out string text))
        {
            infoText.text = text;
            infoButton.gameObject.SetActive(true);
        }
        else
        {
            infoButton.gameObject.SetActive(false);
            infoPanel.SetActive(false);
        }
    }
}