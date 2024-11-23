using System.Collections;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject redMinionPrefab;  // Assign the Red Minion prefab in the Inspector
    [SerializeField] private GameObject blueMinionPrefab; // Assign the Blue Minion prefab in the Inspector

    [SerializeField] private float spawnInterval = 0.02f;    // Time between spawns
    [SerializeField] private float waveInterval = 60f;      // Time between waves

    private Vector3 redSpawnPoint = new Vector3(-37, 0, 0); // Red Minion spawn position
    private Vector3 blueSpawnPoint = new Vector3(37, 0, 0); // Blue Minion spawn position

    private int minionsPerWave = 6; // Total minions per team in a wave

    void Start()
    {
        StartCoroutine(SpawnMinionWaves());
    }

    private IEnumerator SpawnMinionWaves()
    {
        while (true) // Continuously spawn waves
        {
            yield return StartCoroutine(SpawnMinions());
            yield return new WaitForSeconds(waveInterval); // Wait for the next wave
        }
    }

    private IEnumerator SpawnMinions()
    {
        for (int i = 0; i < minionsPerWave; i++)
        {
            // Determine if the minion is a caster
            bool isCaster = i >= 3;

            // Spawn a Red Minion
            SpawnMinion(redMinionPrefab, redSpawnPoint, isCaster);

            // Spawn a Blue Minion
            SpawnMinion(blueMinionPrefab, blueSpawnPoint, isCaster);

            // Adjust spawn positions for the next minions
            redSpawnPoint.x += 0.6f;
            blueSpawnPoint.x -= 0.6f;

            yield return new WaitForSeconds(spawnInterval);
        }

        // Reset spawn positions for the next wave
        redSpawnPoint.x = -37;
        blueSpawnPoint.x = 37;
    }

    private void SpawnMinion(GameObject prefab, Vector3 spawnPosition, bool isCaster)
    {
        GameObject minion = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Set the isCaster property on the spawned minion
        MinionsMovement minionScript = minion.GetComponent<MinionsMovement>();
        if (minionScript != null)
        {
            minionScript.isCaster = isCaster;
        }
    }
}
