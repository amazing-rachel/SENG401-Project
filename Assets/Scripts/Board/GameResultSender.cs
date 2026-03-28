using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class GameResultSender : MonoBehaviour
{
    public string apiURL = "https://game-login.onrender.com/save_results";

    public void SendResult(string username, string result)
    {
        StartCoroutine(PostResult(username, result));
    }

    IEnumerator PostResult(string username, string result)
    {
        string json = "{\"username\":\"" + username + "\",\"result\":\"" + result + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiURL, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Game result saved to server");
        }
        else
        {
            Debug.Log("Error sending result: " + request.error);
        }
    }
}
