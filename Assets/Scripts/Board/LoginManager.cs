using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Login Panel Inputs")]
    public TMP_InputField LoginUsernameInput;
    public TMP_InputField LoginPasswordInput;

    [Header("Create Account Panel Inputs")]
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;

    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject CreateAccountPanel;

    [Header("Backend API")]
    public string api = "https://game-login.onrender.com"; // Render link

    
    // Panel switching
    
    public void ShowCreateAccount()
    {
        LoginPanel.SetActive(false);
        CreateAccountPanel.SetActive(true);
        ClearFields();
    }

    public void ShowLogin()
    {
        LoginPanel.SetActive(true);
        CreateAccountPanel.SetActive(false);
        ClearFields();
    }

    void ClearFields()
    {
        LoginUsernameInput.text = "";
        LoginPasswordInput.text = "";
        RegisterUsernameInput.text = "";
        RegisterPasswordInput.text = "";
    }

    
    // Button actions
    
    public void Login()
    {
        string username = LoginUsernameInput.text;
        string password = LoginPasswordInput.text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogWarning("Login fields cannot be empty!");
            return;
        }

        StartCoroutine(SendRequest("/login", username, password));
    }

    public void Register()
    {
        string username = RegisterUsernameInput.text;
        string password = RegisterPasswordInput.text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogWarning("Register fields cannot be empty!");
            return;
        }

        StartCoroutine(SendRequest("/register", username, password));
    }

    
    // Send POST request to backend
    
    IEnumerator SendRequest(string endpoint, string username, string password)
    {
        string json = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";

        // Debug statements
        Debug.Log("Sending request to: " + api + endpoint);
        Debug.Log("Request body: " + json);


        UnityWebRequest request = new UnityWebRequest(api + endpoint, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;
        Debug.Log("Response from backend: " + response);

        if (response.Contains("success") && endpoint == "/login")
        {
            // Save username for the entire game session
            SessionManager.Instance.CurrentUsername = username;

            Debug.Log("Logged in as: " + username);
            
            SceneManager.LoadScene("OutdoorsScene");
        }
        else if (response.Contains("account_created") && endpoint == "/register")
        {
            Debug.Log("Account created! Returning to login panel.");
            ShowLogin();
        }
        else if (response.Contains("username_taken") && endpoint == "/register")
        {
            Debug.LogWarning("Username already taken!");
        }
        else if (response.Contains("empty_fields"))
        {
            Debug.LogWarning("Username or password cannot be empty!");
        }
        else
        {
            Debug.LogWarning("Request failed: " + response);
        }
    }
}