using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    [SerializeField] public float MinY = -15f;
    [SerializeField] public float MaxVelocity = 15f;
    public float Score = 0;
    [SerializeField] public int Lives = 5;
    public TextMeshProUGUI scoreText;
    public GameObject[] LivesImage;
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
    // --- NEW AUDIO VARIABLE ---
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
            // --- NEW CODE: PLAY LOSE LIFE SOUND ---
            // We play this right before checking for Game Over or resetting
            if (audioSource != null && loseLifeSound != null)
            {
                audioSource.pitch = 1f; // Keep it at normal speed/pitch
                audioSource.PlayOneShot(loseLifeSound, 0.6f); 
            }

            if (Lives <= 0)
            {
                GameOver();
            }
            else
            {
                Lives = Lives - 1;
                if (LivesImage != null && Lives < LivesImage.Length && LivesImage[Lives] != null)
                {
                    LivesImage[Lives].SetActive(false);
                }

                ResetBallPhysics();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ResetBallPhysics()
    {
        transform.position = Vector3.zero;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.down * 12.5f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. HIT A BRICK
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
        // 2. HIT THE PADDLE (PLAYER)
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
        // 3. HIT ANYTHING ELSE (WALLS / CEILING)
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