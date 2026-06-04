using UnityEngine;
using System.Collections;

public class MoveStarsDown : MonoBehaviour {

    [Header("Scrolling Settings")]
    public float scrollSpeed = 0.1f; // Controls how fast the background scrolls down
    public float parallax = 2f;      // Controls how much the background reacts to player movement

    private Material mat;

    void Start() {
        // Cache the material at the start so we aren't calling GetComponent every single frame (better performance!)
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) {
            mat = mr.material;
        }
    }

    void Update () {
        if (mat == null) return;

        // 1. Get the current texture offset
        Vector2 offset = mat.mainTextureOffset;

        // 2. Handle Horizontal Parallax (Moves left/right based on the object's X position)
        offset.x = transform.position.x / transform.localScale.x / parallax;

        // 3. Handle Vertical Scrolling (Constantly moves downward over time)
        // We use "+=" here so it accumulates continuously every frame
        offset.y += scrollSpeed * Time.deltaTime;

        // 4. Apply the updated offset back to the material
        mat.mainTextureOffset = offset;
    }
}