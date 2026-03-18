using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    public string CurrentUsername;
    // used to pass the selected subject to next sence
    public string SelectedSubject;

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
