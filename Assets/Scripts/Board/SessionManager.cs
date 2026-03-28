using UnityEngine;
using System.Collections.Generic;

// Holds data for one play session (login name, subject, question dedup). Lives across scene loads.
public class SessionManager : MonoBehaviour
{
    // One object for the whole game; other scripts use SessionManager.Instance.
    public static SessionManager Instance;

    // Set after login. Used when saving results or showing stats.
    public string CurrentUsername;

    // Which topic the player picked (passed to the game scene).
    public string SelectedSubject;

    // Keys like "subject|question text" so we do not ask the same stem twice in one run. Cleared when the board scene loads.
    public HashSet<string> UsedQuestionKeysThisRun = new HashSet<string>();

    // Remove all keys for one subject only (used when that topic runs out of fresh questions).
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
        // Singleton: keep the first one, delete any extra SessionManager in the scene.
        if (Instance == null)
        {
            Instance = this;
            // Do not destroy when changing scenes so session data stays.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
