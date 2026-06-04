using UnityEngine;
using System.Collections;

public class PlayerCoords : MonoBehaviour
{

    float speed = 2.5f;

    void Update()
    {
        transform.Translate(new Vector3(
            Input.GetAxis("Horizontal") * speed,
            Input.GetAxis("Vertical") * speed,
            0) * Time.deltaTime);
    }
}