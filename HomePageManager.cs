using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePageManager : MonoBehaviour
{
    public void OnLoginButton()
    {
        // Direct to Login
        SceneManager.LoadScene("LoginScene");
    }

    public void OnSignUpButton()
    {
        // Direct to Sign Up
        SceneManager.LoadScene("RegisterScene");
    }
}
