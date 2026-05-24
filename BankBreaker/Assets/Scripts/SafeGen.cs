using UnityEngine;

public class SafeGen : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 offset = new Vector2(1.125f, 0.65f); 
    public GameObject brickPrefab;

    [Header("Industrial Safe Colors")]
    public Color outerFrame = new Color(0.15f, 0.15f, 0.15f); // Near Black
    public Color innerFace = new Color(0.35f, 0.35f, 0.35f);  // Dark Grey
    public Color dialMain = new Color(0.6f, 0.6f, 0.6f);      // Steel Grey
    public Color dialNotch = new Color(0.8f, 0.8f, 0.8f);     // Light Steel
    public Color goldSymbol = new Color(1f, 0.85f, 0f);      // Keeping the Gold $

    // F=Frame (Black), I=Inner (Dark Grey), D=Dial, N=Notch, Y=Gold Symbol
    private string[] layout = new string[]
    {
        "IFFFFFFFFFFFFFFFFFFFFI", // Corners changed to 'I', rest is 'F'
        "FFFFFFFFFFFFFFFFFFFFFF",
        "FIIIIIIIIIIIIIIIIIIIIFF", // Row 2 changed from 'I' to 'F' (Green highlight)
        "FIIFFFFFFFFFFFFFFFFFIFF", // Row 3 changed from 'I' to 'F' (Green highlight)
        "FIIFIIIIINNNNNIIIIIFIFF",
        "FIFFFIIINNNNNNNIIIIFIFF",
        "FIFIFIINNDDDDDNNIIIFIFF",
        "FIFFFINNDDDDDDDNNIIFIFF",
        "FIIFIINNDDYYYYDNNIIFIFF",
        "FIIFIINNDDYYYYDNNIIFIFF",
        "FIIFIINNDDYYDDDNNIIFIFF",
        "FIFFFINNDDDDDDDNNIIFIFF",
        "FIFIFIINNDDDDDNNIIIFIFF",
        "FIFFFIIINNNNNNNIIIIFIFF",
        "FIIFIIIIINNNNNIIIIIFIFF",
        "FIIFIIIIIIIIIIIIIIIFIFF", // Row 15 changed from 'I' to 'F' (Green highlight)
        "FIIFFFFFFFFFFFFFFFFFIFF", // Row 16 changed from 'I' to 'F' (Green highlight)
        "FIIIIIIIIIIIIIIIIIIIIFF",
        "FFFFFFFFFFFFFFFFFFFFFFF",
        "IFFFFFFFFFFFFFFFFFFFFFI", // Bottom corners changed to 'I'
        "  FFF             FFF  "
    };

    private void Awake()
    {
        GenerateSafe();
    }

    private void GenerateSafe()
    {
        int height = layout.Length;
        int width = layout[0].Length;

        float startX = transform.position.x - ((width - 1) * offset.x) / 2f;
        float startY = transform.position.y + ((height - 1) * offset.y) / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x >= layout[y].Length) continue;

                char brickType = layout[y][x];
                if (brickType == ' ') continue;

                GameObject newBrick = Instantiate(brickPrefab, transform);
                newBrick.transform.position = new Vector3(startX + (x * offset.x), startY - (y * offset.y), 0);
                
                SpriteRenderer sr = newBrick.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    switch (brickType)
                    {
                        case 'F': sr.color = outerFrame; break;
                        case 'I': sr.color = innerFace; break;
                        case 'D': sr.color = dialMain; break;
                        case 'N': sr.color = dialNotch; break; 
                        case 'Y': sr.color = goldSymbol; break;
                    }
                }
            }
        }
    }
}