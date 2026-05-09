using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity;
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
    [SerializeField] private int BrickCount;

    Rigidbody2D rb;

    [Header("Juice")]
    [SerializeField] public ParticleSystem _landParticles; //For Bricks
    [SerializeField] private ParticleSystem _jumpParticles; // For Paddle

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.down * 12.5f;
        BrickCount = FindAnyObjectByType<LevelGenBlock>().transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < MinY) // 1. Check if the ball fell below the screen (Game Over / Reset logic)
        {

            if (Lives <= 0)
            {
                GameOver();
            }

            else
            {
                transform.position = Vector3.zero;
                rb.linearVelocity = Vector2.down * 12.5f;
                Lives = Lives - 1;
                LivesImage[Lives].SetActive(false);  
            }
        }

        if (rb.linearVelocity.magnitude > MaxVelocity) // 2. Clamp the speed so the ball doesn't go too fast and fly through walls
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Brick"))
        {
            if (collision.gameObject.TryGetComponent(out SpriteRenderer brickRenderer))
            {
                // Access the main module
                var main = _landParticles.main;

                // We use a MinMaxGradient to ensure it overrides everything
                main.startColor = new ParticleSystem.MinMaxGradient(brickRenderer.color);
            }

            // Move and Play
            _landParticles.transform.position = collision.transform.position;

            // Force a refresh of the particles
            _landParticles.Clear();
            _landParticles.Play();
            // --------------------------

            Destroy(collision.gameObject);
            Score = Score + 10;
            scoreText.text = Score.ToString("00000");

            //------------------------------------------------------------------------------------//
            //BrickCounter Logic

            BrickCount = BrickCount - 1;
            if (BrickCount <= 0)
            {
                WinnerPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
        
        // 2. COLLISION WITH PADDLE (Jump Particles)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Move the particles to the point where the ball touched the paddle
            _jumpParticles.transform.position = collision.contacts[0].point;
            _jumpParticles.Play();
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver");
        GameOverPanel.SetActive(true);
        Time.timeScale = 0;
        Destroy(gameObject);
    }
}