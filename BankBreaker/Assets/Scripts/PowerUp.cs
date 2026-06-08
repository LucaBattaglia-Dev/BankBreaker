using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { ExtraLife, MultiBall, LongPaddle }
    public PowerUpType type;

    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float despawnY = -27.5f;

    void Update()
    {
        // Move downward continuously
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // Despawn if it falls past the threshold
        if (transform.position.y < despawnY)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it hit the Paddle (Player)
        if (other.CompareTag("Player"))
        {
            ActivatePowerUp(other.gameObject);
            Destroy(gameObject); // Destroy the power-up item container
        }
    }

    private void ActivatePowerUp(GameObject paddle)
    {
        // Find the active ball in the scene to modify lives/scores if needed
        Ball ball = FindFirstObjectByType<Ball>();

        switch (type)
        {
            case PowerUpType.ExtraLife:
                if (ball != null && ball.Lives < ball.LivesImage.Length)
                {
                    ball.LivesImage[ball.Lives].SetActive(true);
                    ball.Lives++;
                }
                break;

            case PowerUpType.MultiBall:
                if (ball != null)
                {
                    // Spawn 2 extra balls at the current ball's position
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject newBallObj = Instantiate(ball.gameObject, ball.transform.position, Quaternion.identity);
                        Rigidbody2D newRb = newBallObj.GetComponent<Rigidbody2D>();
                        
                        // Give the new balls a slight randomized diagonal trajectory
                        if (newRb != null)
                        {
                            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
                            newRb.linearVelocity = randomDirection * ball.MaxVelocity;
                        }
                    }
                }
                break;

            case PowerUpType.LongPaddle:
                // Start the scaling routine on the paddle object directly
                PlayerMovement paddleScript = paddle.GetComponent<PlayerMovement>();
                if (paddleScript != null)
                {
                    paddleScript.TriggerLongPaddle(15f);
                }
                else
                {
                    // Fallback if no specific script manages it
                    StartCoroutine(ScalePaddleRoutine(paddle.transform, 15f));
                }
                break;
        }
    }

    // Fallback coroutine if your paddle doesn't have its own scaler script
    private IEnumerator ScalePaddleRoutine(Transform paddleTransform, float duration)
    {
        // We assume the baseline default scale of your paddle on X is what it started with.
        // If it's already modified, we trace back or apply a hard limit relative to a normal scale of 1.
        float normalScaleX = 1f; 
        float maxScaleX = normalScaleX * 3.5f;

        Vector3 currentScale = paddleTransform.localScale;
        
        // Calculate the target scale (double the current width)
        float targetScaleX = currentScale.x * 2f;

        // Clamp the new target scale so it never exceeds 3.5x normal size
        if (targetScaleX > maxScaleX)
        {
            targetScaleX = maxScaleX;
        }

        // Apply the new scale
        paddleTransform.localScale = new Vector3(targetScaleX, currentScale.y, currentScale.z);

        yield return new WaitForSeconds(duration);

        // Reset completely back to normal size when the 15 seconds are up
        if (paddleTransform != null)
        {
            paddleTransform.localScale = new Vector3(normalScaleX, currentScale.y, currentScale.z);
        }
    }
}