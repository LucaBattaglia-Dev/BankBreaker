using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float Speed = 5;
    [SerializeField] public float MaxDistance = 7f;
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
        //Debug.Log("Game Updating");
        MovementHorizontal = Input.GetAxis("Horizontal");
        HandleParticles(MovementHorizontal);

        if ((MovementHorizontal > 0 && transform.position.x < MaxDistance) || (MovementHorizontal < 0 && transform.position.x > -MaxDistance))
        {
            // Moves the player using Vector3.right multiplied by movement direction
            transform.position += Vector3.right * MovementHorizontal * Speed * Time.deltaTime;
        }
    }
    
    void HandleParticles(float input)
    {
        if (input > 0)
        {
            if (!_moveParticlesRight.isEmitting) _moveParticlesRight.Play();
            _moveParticlesLeft.Stop();
        }
        // 2. Logic for Moving Left
        else if (input < 0)
        {
            if (!_moveParticlesLeft.isEmitting) _moveParticlesLeft.Play();
            _moveParticlesRight.Stop();
        }
        // 3. Logic for Standing Still
        else
        {
            _moveParticlesRight.Stop();
            _moveParticlesLeft.Stop();
        }

        // float targetScale = Mathf.Abs(MovementHorizontal) > 0 ? 1f : 0.2f;
        // _moveParticles.transform.localScale = Vector3.Lerp(_moveParticles.transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5f);
    }
}
