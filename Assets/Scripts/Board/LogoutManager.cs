using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    // Return to Login scene if Logout button is pressed
    public void PerformLogout()
    {
        SceneManager.LoadScene("LoginScene"); 
    }
}