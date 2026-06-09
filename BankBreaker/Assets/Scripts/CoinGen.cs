using UnityEngine;

public class CoinGen : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 offset = new Vector2(1.125f, 0.65f); 
    public GameObject brickPrefab;

    [Header("Shades of Gold")]
    public Color coinRim = new Color(0.65f, 0.45f, 0.0f);     // Dark Gold / Bronze for the outer shadow border
    public Color coinFace = new Color(0.95f, 0.75f, 0.0f);    // Bright Main Gold for the base of the coin
    public Color coinHighlight = new Color(1.0f, 0.92f, 0.5f); // Light/Pale Gold for reflections and highlights
    public Color symbolColor = new Color(0.45f, 0.3f, 0.0f);   // Deep Brown/Gold to make the '$' pop out in 3D

    // R = Rim, F = Face, H = Highlight, S = Dollar Symbol, ' ' = Transparent/Empty Space
    private string[] layout = new string[]
    {
        "    RRRRRRR    ",
        "  RRRHHHHHRRR  ",
        " RRHHHHHHHHHRR ",
        "RRHHHFFFFFHHHRR",
        "RHHHFFSSSFHHHHR",
        "RHHFFSSFSSFHFFR",
        "RHHHFFSSFFFFHFR",
        "RHHHHHFFFSSFHFR",
        "RHFFFSSFSSFHFFR",
        "RFFFFFSSSFFFFFR",
        "RRFFFFFFFFFFFRR",
        " RRFFFFFFFFFRR ",
        "  RRRFFFFFRRR  ",
        "    RRRRRRR    "
    };

    private void Awake()
    {
        GenerateCoin();
    }

    private void GenerateCoin()
    {
        int height = layout.Length;
        int width = layout[0].Length;

        // Centers the coin generation perfectly on the GameObject's position
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
                        case 'R': sr.color = coinRim; break;
                        case 'F': sr.color = coinFace; break;
                        case 'H': sr.color = coinHighlight; break; 
                        case 'S': sr.color = symbolColor; break;
                    }
                }
            }
        }
    }
}