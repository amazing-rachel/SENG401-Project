using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PostGameUIManager : MonoBehaviour
{
    public GameObject statsPanel;      // Assign StatsPanel
    public TMP_Text statsText;             // Assign StatsText

    public ResultManager resultManager;

    void Awake()
    {
        statsPanel.SetActive(false);   
    }

    public void ShowStats()
    {
        statsPanel.SetActive(true);
        StartCoroutine(LoadStats());
    }

    public void HideStats()
    {
        statsPanel.SetActive(false);
    }

    IEnumerator LoadStats()
    {
        string username = SessionManager.Instance.CurrentUsername;

        string url = $"https://game-login.onrender.com/stats/{username}";
        Debug.Log("Fetching stats from: " + url);

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var json = www.downloadHandler.text;
                statsText.text = "Your Stats:\n" + json;
            }
            else
            {
                statsText.text = "Error loading stats";
                Debug.Log("Error getting stats: " + www.error);
            }
        }
    }

    public void BackToLogin()
    {
        SceneManager.LoadScene("LoginScene"); 
    }

    public void PlayAgain()
    {
        statsPanel.SetActive(false);
        resultManager.ResetBoard();
    }
}
