using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cashGen : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 offset = new Vector2(1.125f, 0.625f); 
    public GameObject brickPrefab;

    [Header("Cash Colors")]
    public Color darkGreen = new Color(0.12f, 0.30f, 0.15f, 1f);       // Left Border
    public Color interiorLeft = new Color(0.25f, 0.55f, 0.30f, 1f);    // Left Interior
    public Color mediumGreen = new Color(0.05f, 0.15f, 0.08f, 1f);     // Right Border (Even Darker)
    public Color interiorRight = new Color(0.45f, 0.75f, 0.50f, 1f);   // Right Interior
    public Color yellowGold = new Color(0.95f, 0.80f, 0.20f, 1f);      // $ Symbol

    // Each row MUST be exactly 32 characters long
    private string[] layout = new string[]
    {
        "DDDDDDDDDDDDDDDDMMMMMMMMMMMMMMM", // 0
        "DDIIIIIIIIIIIIIILLLLLLLLLLLLLMM", // 1
        "DDIIIIIIIIIDDDDDMMMMLLLLLLLLLMM", // 2
        "DDIIIIIIIDDDLLLYLLLMMMLLLLLLLMM", // 3
        "DDIIIIIIDDLLLLYYYLLLLMMLLLLLLMM", // 4
        "DDIIYIIDDLLLLYLYLLLLLLMMLLYLLMM", // 5
        "DDIYYYIDDLLLLLYYYLLLLLMMLYYYLMM", // 6 (Symmetry Center)
        "DDIIYIIDDLLLLLLYLYLLLLMMLLYLLMM", // 7
        "DDIIIIIIDDLLLLYYYLLLLMMLLLLLLMM", // 8
        "DDIIIIIIIDDDLLLYLLLMMMLLLLLLLMM", // 9
        "DDIIIIIIIIIDDDDDMMMMLLLLLLLLLMM", // 10
        "DDIIIIIIIIIIIIIILLLLLLLLLLLLLMM", // 11
        "DDDDDDDDDDDDDDDDMMMMMMMMMMMMMMM"  // 12
    };

    private void Awake()
    {
        GenerateCash();
    }

    private void GenerateCash()
    {
        int height = layout.Length;
        int width = 32; // Standardized width

        float startX = transform.position.x - ((width - 1) * offset.x) / 2f;
        float startY = transform.position.y + ((height - 1) * offset.y) / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // FAIL SAFE: Check if the current row actually has a character at this 'x'
                if (x >= layout[y].Length) continue;

                char brickType = layout[y][x];
                if (brickType == ' ') continue;

                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.transform.position = new Vector3(startX + (x * offset.x), startY - (y * offset.y), 0);
                newBrick.name = $"Brick_{y}_{x}";

                SpriteRenderer sr = newBrick.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    switch (brickType)
                    {
                        case 'D': sr.color = darkGreen; break;
                        case 'I': sr.color = interiorLeft; break;
                        case 'M': sr.color = mediumGreen; break;
                        case 'L': sr.color = interiorRight; break;
                        case 'Y': sr.color = yellowGold; break;
                    }
                }
            }
        }
    }
}