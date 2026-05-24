using UnityEngine;
using UnityEngine.SceneManagement; // Required for restarting scenes

public class GameManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject PauseMenu;
    
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    // --- New Functions ---

    public void RestartGame()
    {
        // Crucial: Reset time speed so the new game isn't frozen!
        Time.timeScale = 1f;
        
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested"); // So you can see it working in the editor
        Application.Quit();
    }
}