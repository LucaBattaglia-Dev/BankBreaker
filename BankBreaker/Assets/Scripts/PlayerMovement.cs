using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float Speed = 5f;
    [SerializeField] public float MaxDistance = 10f; 

    private float MovementHorizontal;

    [Header("Movement Particles")]
    [SerializeField] private ParticleSystem _moveParticlesLeft;  // Drag the LEFT particle system here
    [SerializeField] private ParticleSystem _moveParticlesRight; // Drag the RIGHT particle system here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game Started");
    }

    // Update is called once per frame
    void Update()
    {
        // 1. Get Horizontal Input Only
        MovementHorizontal = Input.GetAxis("Horizontal");

        // 2. Handle Particles based on horizontal movement
        HandleParticles(MovementHorizontal);

        // 3. Horizontal Movement Only (Clamped)
        if ((MovementHorizontal > 0 && transform.position.x < MaxDistance) || (MovementHorizontal < 0 && transform.position.x > -MaxDistance))
        {
            transform.position += Vector3.right * MovementHorizontal * Speed * Time.deltaTime;
        }
    }
    private Coroutine paddleScaleCoroutine;
    private float baseScaleX = -1f; // Cache the real starting size

    public void TriggerLongPaddle(float duration)
    {
        // Record the absolute original scale the very first time a powerup is grabbed
        if (baseScaleX < 0) baseScaleX = transform.localScale.x;

        if (paddleScaleCoroutine != null) StopCoroutine(paddleScaleCoroutine);
        paddleScaleCoroutine = StartCoroutine(LongPaddleRoutine(duration));
    }

    private IEnumerator LongPaddleRoutine(float duration)
    {
        Vector3 currentScale = transform.localScale;
        
        // Double it, but cap it firmly at 3.5x the base scale
        float newScaleX = Mathf.Min(currentScale.x * 2f, baseScaleX * 3.5f);
        
        transform.localScale = new Vector3(newScaleX, currentScale.y, currentScale.z);
        
        yield return new WaitForSeconds(duration);
        
        // Return to original base size
        transform.localScale = new Vector3(baseScaleX, currentScale.y, currentScale.z);
    }
    
    void HandleParticles(float input)
    {
        if (input > 0)
        {
            if (!_moveParticlesRight.isEmitting) _moveParticlesRight.Play();
            _moveParticlesLeft.Stop();
        }
        // Logic for Moving Left
        else if (input < 0)
        {
            if (!_moveParticlesLeft.isEmitting) _moveParticlesLeft.Play();
            _moveParticlesRight.Stop();
        }
        // Logic for Standing Still
        else
        {
            _moveParticlesRight.Stop();
            _moveParticlesLeft.Stop();
        }
    }
}