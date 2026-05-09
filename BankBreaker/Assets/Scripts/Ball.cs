using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Ball : MonoBehaviour
{
    [SerializeField] public float MinY = -15f;
    [SerializeField] public float MaxVelocity = 15f;
    public float Score = 0;
    [SerializeField] public float Lives = 5;

    Rigidbody2D rb;


    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < MinY) // 1. Check if the ball fell below the screen (Game Over / Reset logic)
        {
            transform.position = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            Lives = Lives - 1;
        }

        if (rb.linearVelocity.magnitude > MaxVelocity) // 2. Clamp the speed so the ball doesn't go too fast and fly through walls
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            Score = Score + 10;
        }
    }
}