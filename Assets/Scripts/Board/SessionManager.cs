using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    public string CurrentUsername;

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
