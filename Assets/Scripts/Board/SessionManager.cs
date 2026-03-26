using UnityEngine;
using System.Collections.Generic;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    public string CurrentUsername;
    // used to pass the selected subject to next sence
    public string SelectedSubject;

    /// <summary>
    /// One playthrough of OutdoorsScene: normalized "subject|question" keys already shown (any difficulty).
    /// Cleared when QuestionManager loads the game scene.
    /// </summary>
    public HashSet<string> UsedQuestionKeysThisRun = new HashSet<string>();

    public void ClearQuestionKeysForSubject(string subject)
    {
        if (string.IsNullOrEmpty(subject) || UsedQuestionKeysThisRun.Count == 0)
            return;
        string prefix = subject.Trim().ToLowerInvariant() + "|";
        var toRemove = new List<string>();
        foreach (string k in UsedQuestionKeysThisRun)
        {
            if (k.StartsWith(prefix))
                toRemove.Add(k);
        }
        foreach (string k in toRemove)
            UsedQuestionKeysThisRun.Remove(k);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
