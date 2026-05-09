using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float Speed = 5;
    [SerializeField] public float MaxDistance = 7f;
    private float MovementHorizontal;

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
        if ((MovementHorizontal > 0 && transform.position.x < MaxDistance) || (MovementHorizontal < 0 && transform.position.x > -MaxDistance))
        {
            // Moves the player using Vector3.right multiplied by movement direction
            transform.position += Vector3.right * MovementHorizontal * Speed * Time.deltaTime;
        }
    }
}
