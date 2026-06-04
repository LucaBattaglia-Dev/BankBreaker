using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Pause Menu")]
    public GameObject PauseMenu;
    private bool isPaused = false;

    // Tracks which level indices (0, 1, 2) have already been completed
    [HideInInspector] public List<int> beatenLevels = new List<int>();

    void Awake()
    {
        // Keeps this script alive across internal loops
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (PauseMenu != null && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        if (PauseMenu == null) return;
        PauseMenu.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    public void PauseGame()
    {
        if (PauseMenu == null) return;
        PauseMenu.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        beatenLevels.Clear();
        // Reloads whatever scene you are currently in
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested");
        Application.Quit();
    }

    public bool CompleteCurrentLevel(int levelIndex)
    {
        if (!beatenLevels.Contains(levelIndex))
        {
            beatenLevels.Add(levelIndex);
        }

        if (beatenLevels.Count >= 3)
        {
            Debug.Log("Victory! All 3 level prefabs beaten!");
            return true; 
        }

        return false; 
    }
}