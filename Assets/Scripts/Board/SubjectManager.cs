using UnityEngine;
using UnityEngine.SceneManagement;

public class SubjectManager : MonoBehaviour {

    public void ChooseMath()
    {
        //SessionManager.Instance.CurrentSubject = "Math";
        SceneManager.LoadScene("OutdoorsScene");
    }

    public void ChooseScience()
    {
        //SessionManager.Instance.CurrentSubject = "Science";
        SceneManager.LoadScene("OutdoorsScene");
    }

    public void ChooseEnglish()
    {
        //SessionManager.Instance.CurrentSubject = "English";
        SceneManager.LoadScene("OutdoorsScene");
    }

    public void ChooseGlobalCitizenship()
    {
        //SessionManager.Instance.CurrentSubject = "Global Citizenship";
        SceneManager.LoadScene("OutdoorsScene");
    }
}