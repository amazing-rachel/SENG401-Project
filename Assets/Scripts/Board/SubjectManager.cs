using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubjectManager : MonoBehaviour {

    [Header("Subject Buttons")]
    public Button MathButton;
    public Button ScienceButton;
    public Button EnglishButton;
    public Button GlobalCitizenshipButton;

    private string chosenSubject = ""; // fix later

    Color32 selectedColor = new Color32(145, 220, 106, 255); 
    Color32 normalColor = new Color32(255, 255, 255, 255); 

    public void ResetButtonColours() {
        MathButton.image.color = normalColor;
        ScienceButton.image.color = normalColor;
        EnglishButton.image.color = normalColor;
        GlobalCitizenshipButton.image.color = normalColor;
    }

    public void ChooseMath() {
        chosenSubject = "Math & Logic";
        ResetButtonColours();
        MathButton.image.color = selectedColor;
        Debug.Log("Chosen subject = " + chosenSubject);    
    }

    public void ChooseScience() {
        chosenSubject = "Environmental Science";
        ResetButtonColours();
        ScienceButton.image.color = selectedColor;
        Debug.Log("Chosen subject = " + chosenSubject);      
    }

    public void ChooseEnglish() {
        chosenSubject = "English Grammar";
        ResetButtonColours();
        EnglishButton.image.color = selectedColor;
        Debug.Log("Chosen subject = " + chosenSubject);      
    }

    public void ChooseGlobalCitizenship() {
        chosenSubject = "Global Citizenship";
        ResetButtonColours();
        GlobalCitizenshipButton.image.color = selectedColor;
        Debug.Log("Chosen subject = " + chosenSubject);  
    }

    public void Play(){
        if (chosenSubject == "") {
            Debug.Log("Subject hasn't been chosen yet!");
            return;
        }
        SessionManager.Instance.SelectedSubject = chosenSubject;
        
        Debug.Log("Loading game with subject: " + chosenSubject);
        SceneManager.LoadScene("OutdoorsScene");    
    }
}
