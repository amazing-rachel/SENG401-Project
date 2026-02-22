using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginSceneManager : MonoBehaviour
{
    public InputField userNameField;
    public InputField passwordField;
    public Button loginButton;
    public Button backButton;

    void Start()
    {
        // Set up buttons
        loginButton.onClick.AddListener(ProcessLogin);
        backButton.onClick.AddListener(GoBack);
    }

    void ProcessLogin()
    {
        string user = userNameField.text;
        string pass = passwordField.text;

        // Validate that fields aren't empty
        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
        {
            Debug.Log("Login Attempt: " + user);
            
            // To implement next: check login details from database
            
            // Successful Login:
            SceneManager.LoadScene("MainMenu"); 
        }
    }

    void GoBack()
    {
        // Goes back to home page
        SceneManager.LoadScene("HomeScene");
    }
}
