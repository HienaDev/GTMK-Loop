using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Reloads the currently active scene
    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Loads the scene named "MainMenu"
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
