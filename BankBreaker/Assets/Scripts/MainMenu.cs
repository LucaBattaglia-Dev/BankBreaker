using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    // Call this function from your UI Button component
    public void LoadGameScene()
    {
        Time.timeScale = 1f; // Make sure time isn't frozen
        SceneManager.LoadScene("Level 1");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested");
        Application.Quit();
    }
}
