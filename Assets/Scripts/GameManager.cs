using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject redMinionPrefab;  // Assign the Red Minion prefab in the Inspector
    [SerializeField] private GameObject blueMinionPrefab; // Assign the Blue Minion prefab in the Inspector

    [SerializeField] private float spawnInterval = 2f;    // Time between spawns

    private Vector3 redSpawnPoint = new Vector3(-7, 0, 0); // Red Minion spawn position
    private Vector3 blueSpawnPoint = new Vector3(7, 0, 0); // Blue Minion spawn position

    private int minionsToSpawn = 3; // Number of minions to spawn for each team

    void Start()
    {
        StartCoroutine(SpawnMinions());
    }

    private IEnumerator SpawnMinions()
    {
        for (int i = 0; i < minionsToSpawn; i++)
        {
            // Spawn a Red Minion
            Instantiate(redMinionPrefab, redSpawnPoint, Quaternion.identity);

            // Spawn a Blue Minion
            Instantiate(blueMinionPrefab, blueSpawnPoint, Quaternion.identity);

            redSpawnPoint.x += 0.6f; // Adjust the spawn position for the next minion
            blueSpawnPoint.x -= 0.6f; // Adjust the spawn position for the next minion
            // Wait for the next spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
