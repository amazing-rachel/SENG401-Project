using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

// Login and sign-up UI. Sends username and password to the backend (HTTPS).
public class LoginManager : MonoBehaviour
{

    [Header("UI Title")]
    public TextMeshProUGUI GameTitle;

    [Header("Login Panel Inputs")]
    public TMP_InputField LoginUsernameInput;
    public TMP_InputField LoginPasswordInput;

    [Header("Create Account Panel Inputs")]
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;

    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject CreateAccountPanel;
    public GameObject SubjectPanel;

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

    // After a good login, show the subject choice screen.
    public void ShowSubjects()
    {
        LoginPanel.SetActive(false);
        CreateAccountPanel.SetActive(false);
        SubjectPanel.SetActive(true);
        ClearFields();
    }

    // Clear every text field so old input does not stay on screen.
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

        // Do not call the server if fields are blank.
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Debug.Log("Login fields cannot be empty!");
            return;
        }

        StartCoroutine(SendRequest("/login", username, password));
    }

    public void Register()
    {
        string username = RegisterUsernameInput.text;
        string password = RegisterPasswordInput.text;

        // Do not call the server if fields are blank.
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Debug.Log("Register fields cannot be empty!");
            return;
        }

        StartCoroutine(SendRequest("/register", username, password));
    }

    
    // Send POST request to backend
    
    IEnumerator SendRequest(string endpoint, string username, string password)
    {
        string json = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";

        Debug.Log("Sending request to: " + api + endpoint);
        Debug.Log("Request body: " + json);

        UnityWebRequest request = new UnityWebRequest(api + endpoint, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        // Tell the server the body is JSON.
        request.SetRequestHeader("Content-Type", "application/json");
        // Wait at most 60 seconds before giving up.
        request.timeout = 60;

        yield return request.SendWebRequest();

        string response = request.downloadHandler != null ? request.downloadHandler.text : "";

        // Network error or bad HTTP status.
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "HTTP/network error — result=" + request.result +
                " httpCode=" + request.responseCode +
                " error=" + request.error +
                " body=" + response);

            request.Dispose();
            yield break;
        }

        Debug.Log("Response from backend (" + request.responseCode + "): " + response);
        request.Dispose();

        // Backend sends short text flags in the body; we branch on those.
        if (response.Contains("success") && endpoint == "/login")
        {
            // Remember who is logged in for the rest of the game.
            SessionManager.Instance.CurrentUsername = username;

            Debug.Log("Logged in as: " + username);
            
            ShowSubjects();
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