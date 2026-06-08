using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // A static reference ensures this variable is shared across all copies of the script,
    // allowing us to track if the music object has already been marked as permanent.
    private static MainMenu musicInstance;

    [Header("Background Music")]
    [Tooltip("Drag your background music loop clip here")]
    [SerializeField] private AudioClip backgroundMusic;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    private AudioSource audioSource;

    void Awake()
    {
        // --- PERSISTENT MUSIC LOGIC ---
        // If an instance already exists and it's not this one, a duplicate menu scene was loaded.
        // We only destroy this specific script instance component if it's attached to a UI canvas/button,
        // so we don't destroy your button layout, but we also don't play double music.
        if (musicInstance != null && musicInstance != this)
        {
            // If this object is just a temporary UI element and the music is already running elsewhere,
            // we don't need to initialize music on it.
            return;
        }

        // If this is the very first time the main menu loads, claim the music instance slot
        if (musicInstance == null)
        {
            musicInstance = this;
            
            // Tell Unity not to destroy this specific GameObject when changing scenes
            DontDestroyOnLoad(gameObject);

            // Set up and play the looping background audio track
            SetupBackgroundMusic();
        }
    }

    private void SetupBackgroundMusic()
    {
        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = musicVolume;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }
    }

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