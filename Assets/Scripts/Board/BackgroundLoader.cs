using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("BackgroundScene", LoadSceneMode.Additive);
    }
}