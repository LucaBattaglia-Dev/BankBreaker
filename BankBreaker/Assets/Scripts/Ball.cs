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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetBallPhysics();
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
            if (Lives <= 0)
            {
                GameOver();
            }
            else
            {
                ResetBallPhysics();
                Lives = Lives - 1;
                LivesImage[Lives].SetActive(false);
            }
        }

        if (rb.linearVelocity.magnitude > MaxVelocity)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
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

            Destroy(collision.gameObject);
            Score = Score + 10;
            if (scoreText != null) scoreText.text = Score.ToString("00000");

            BrickCount--;
            if (BrickCount <= 0)
            {
                HandleLevelClear();
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            _jumpParticles.transform.position = collision.contacts[0].point;
            _jumpParticles.Play();
        }
    }

    void HandleLevelClear()
    {
        if (GameManager.Instance != null)
        {
            // Complete current level and verify if it was the definitive final layout match
            bool isGameFinished = GameManager.Instance.CompleteCurrentLevel(currentLevelIndex);

            if (isGameFinished)
            {
                // Absolute completion! Freeze space calculations and load the victory splash screen panel
                rb.linearVelocity = Vector2.zero;
                if (WinnerPanel != null) WinnerPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                // There are still layouts left! Instantly loop in the next prefab configuration in-scene
                ResetBallPhysics();
                if (LevelLoader.Instance != null)
                {
                    LevelLoader.Instance.SpawnNextRandomLevel();
                }
            }
        }
        else
        {
            // Fallback emergency option if testing standalone
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
