using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public void PerformLogout()
    {
        SceneManager.LoadScene("LoginScene"); 
    }
}