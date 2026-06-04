using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("Assign your 3 Level Prefabs here")]
    [SerializeField] private GameObject[] levelPrefabs; 

    private GameObject currentSpawnedLevel;

    void Awake()
    {
        // Simple instance lookup so the Ball script can easily find it
        Instance = this;
    }

    void Start()
    {
        // Spawn the very first layout when the scene loads
        SpawnNextRandomLevel();
    }

    public void SpawnNextRandomLevel()
    {
        if (GameManager.Instance == null) return;

        // 1. Clean up the old level layout if it exists
        if (currentSpawnedLevel != null)
        {
            Destroy(currentSpawnedLevel);
        }

        List<int> availableIndices = new List<int>();

        // 2. Filter out already beaten prefab indices
        for (int i = 0; i < levelPrefabs.Length; i++)
        {
            if (!GameManager.Instance.beatenLevels.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        // 3. Spawn the new prefab if any are left
        if (availableIndices.Count > 0)
        {
            int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            
            currentSpawnedLevel = Instantiate(levelPrefabs[randomIndex], transform.position, transform.rotation);
            
            // Collect and count structural bricks natively inside the new root container
            List<GameObject> actualBricks = new List<GameObject>();
            foreach (Transform child in currentSpawnedLevel.transform)
            {
                if (child.CompareTag("Brick")) actualBricks.Add(child.gameObject);
                foreach (Transform subChild in child)
                {
                    if (subChild.CompareTag("Brick") && !actualBricks.Contains(subChild.gameObject))
                    {
                        actualBricks.Add(subChild.gameObject);
                    }
                }
            }

            int finalBrickCount = actualBricks.Count;
            Debug.Log($"Spawned Prefab Index: {randomIndex} with {finalBrickCount} bricks.");

            // 4. Update the ball details
            Ball gameBall = FindFirstObjectByType<Ball>();
            if (gameBall != null)
            {
                gameBall.InitializeLevelTrack(randomIndex, finalBrickCount);
            }

            // Skip safeguard
            if (finalBrickCount <= 0)
            {
                Debug.LogWarning($"Skipping empty layout prefab: {randomIndex}");
                GameManager.Instance.CompleteCurrentLevel(randomIndex);
                SpawnNextRandomLevel();
            }
        }
    }
}