using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    [SerializeField] public float MinY = -15f;
    [SerializeField] public float MaxY = 20f;
    [SerializeField] public float MaxVelocity = 15f;
    public float Score = 0;
    
    [Header("Life System Settings")]
    [SerializeField] public int Lives = 3;             // Starts at 3
    private const int MAX_LIVES = 5;                  // Absolute maximum limit
    
    public TextMeshProUGUI scoreText;
    public GameObject[] LivesImage;                  // Assumed size of 5 elements in Inspector
    public GameObject GameOverPanel;
    public GameObject WinnerPanel;

    [Header("Current Level Status")]
    public int BrickCount;
    private int currentLevelIndex = -1;

    Rigidbody2D rb;

    [Header("Juice")]
    [SerializeField] public ParticleSystem _landParticles;
    [SerializeField] private ParticleSystem _jumpParticles;
    
    [Header("Audio")]
    [Tooltip("Drag your BlockHit audio clip here")]
    [SerializeField] private AudioClip blockHit; 
    [Tooltip("Drag your Paddle/Wall bounce audio clip here")]
    [SerializeField] private AudioClip paddleSound; 
    [Tooltip("Drag your Lose Life audio clip here")]
    [SerializeField] private AudioClip loseLifeSound; 
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // --- NEW INITIALIZATION CODE ---
        // Ensure starting lives don't accidentally exceed the hard cap
        if (Lives > MAX_LIVES) Lives = MAX_LIVES;

        // Sync UI at startup: If we start with 3 lives, deactivate index 3 and 4 (hearts 4 and 5)
        if (LivesImage != null)
        {
            for (int i = 0; i < LivesImage.Length; i++)
            {
                if (LivesImage[i] != null)
                {
                    // Only activate up to the current starting Lives count
                    LivesImage[i].SetActive(i < Lives);
                }
            }
        }
        
        Ball[] allBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        if (allBalls.Length <= 1)
        {
            ResetBallPhysics();
        }
    }

    public void InitializeLevelTrack(int index, int totalBricks)
    {
        currentLevelIndex = index;
        BrickCount = totalBricks;
    }

    void Update()
    {
        if (transform.position.y < MinY)
        {
            HandleBallLoss();
            return; 
        }

        if (transform.position.y > MaxY)
        {
            HandleBallLoss();
            return;
        }

        if (rb.linearVelocity.magnitude > MaxVelocity)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
        }
    }

    void HandleBallLoss()
    {
        Ball[] activeBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);

        if (activeBalls.Length <= 1)
        {
            if (audioSource != null && loseLifeSound != null)
            {
                audioSource.pitch = 1f; 
                audioSource.PlayOneShot(loseLifeSound, 0.6f); 
            }

            // Lose a life first so our tracking matches the array logic perfectly
            Lives--;

            // Turn off the heart icon corresponding to the lost life slot
            if (LivesImage != null && Lives >= 0 && Lives < LivesImage.Length && LivesImage[Lives] != null)
            {
                LivesImage[Lives].SetActive(false);
            }

            if (Lives <= 0)
            {
                GameOver();
            }
            else
            {
                ResetBallPhysics();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method from your Extra Life PowerUp script instead of directly editing variables!
    public bool TryGainExtraLife()
    {
        if (Lives >= MAX_LIVES) return false; // Firmly block gaining more than 5 lives

        if (LivesImage != null && Lives < LivesImage.Length && LivesImage[Lives] != null)
        {
            LivesImage[Lives].SetActive(true);
        }
        
        Lives++;
        return true;
    }

    void ResetBallPhysics()
    {
        // --- NEW TRAIL RESET CODE ---
        // Find the TrailRenderer component on this ball
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();          // Instantly wipes away any existing trail points
            trail.enabled = false;  // Disables the trail so it stops drawing temporarily
        }

        // Teleport the ball back to the center
        transform.position = Vector3.zero;
        
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.down * 12.5f;

        // --- RE-ENABLE TRAIL AFTER TELEPORT ---
        if (trail != null)
        {
            trail.enabled = true;   // Turns the trail back on now that the ball is at (0,0)
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            if (collision.gameObject.TryGetComponent(out SpriteRenderer brickRenderer))
            {
                var main = _landParticles.main;
                main.startColor = new ParticleSystem.MinMaxGradient(brickRenderer.color);
            }

            _landParticles.transform.position = collision.transform.position;
            _landParticles.Clear();
            _landParticles.Play();

            if (audioSource != null && blockHit != null)
            {
                audioSource.pitch = 1.25f; 
                audioSource.PlayOneShot(blockHit, 0.4f);
            }

            Destroy(collision.gameObject);
            Score = Score + 10;
            
            if (scoreText != null) scoreText.text = Score.ToString("00000");

            Ball[] activeBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
            foreach (Ball b in activeBalls)
            {
                b.BrickCount--;
                if (b.BrickCount <= 0)
                {
                    b.HandleLevelClear();
                }
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            _jumpParticles.transform.position = collision.contacts[0].point;
            _jumpParticles.Play();

            if (audioSource != null && paddleSound != null)
            {
                audioSource.pitch = 2f; 
                audioSource.PlayOneShot(paddleSound, 0.5f);
            }
        }
        else
        {
            if (audioSource != null && paddleSound != null)
            {
                audioSource.pitch = 2f; 
                audioSource.PlayOneShot(paddleSound, 0.4f);
            }
        }
    }

    void HandleLevelClear()
    {
        if (GameManager.Instance != null)
        {
            bool isGameFinished = GameManager.Instance.CompleteCurrentLevel(currentLevelIndex);

            if (isGameFinished)
            {
                rb.linearVelocity = Vector2.zero;
                if (WinnerPanel != null) WinnerPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                if (LevelLoader.Instance != null)
                {
                    LevelLoader.Instance.SpawnNextRandomLevel();
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (WinnerPanel != null) WinnerPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void GameOver()
    {
        GameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Destroy(gameObject);
    }
}