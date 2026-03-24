using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubjectInfoUI : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infoText;
    public Button infoButton; 
    public Button closeInfoBtn;

    private QuestionManager qm;

    void Awake()
    {
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