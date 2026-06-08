using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Power-up Drop settings")]
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.20f; // 10% chance
    [SerializeField] private GameObject[] powerUpPrefabs; // Array of your 3 power-up prefabs

    // This gets called right when the object is destroyed
    void OnDestroy()
    {
        // Safety check to ensure we don't spawn powerups when stopping the game player scene
        if (!gameObject.scene.isLoaded) return;

        // Roll the dice (0.0 to 1.0). If less than 0.10, trigger drop
        if (Random.value <= dropChance && powerUpPrefabs != null && powerUpPrefabs.Length > 0)
        {
            // Pick a random power-up index from your array
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            
            if (powerUpPrefabs[randomIndex] != null)
            {
                Instantiate(powerUpPrefabs[randomIndex], transform.position, Quaternion.identity);
            }
        }
    }
}