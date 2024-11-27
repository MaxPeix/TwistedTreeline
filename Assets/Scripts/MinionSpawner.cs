using System.Collections;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject redMinionPrefab;  // Assign the Red Minion prefab in the Inspector
    [SerializeField] private GameObject blueMinionPrefab; // Assign the Blue Minion prefab in the Inspector

    [SerializeField] private float spawnInterval = 0.02f;    // Time between spawns
    [SerializeField] private float waveInterval = 60f;      // Time between waves

    public GameObject[]BlueTopWaypoints;
    public GameObject[]BlueBotWaypoints;
    private GameObject[]RedTopWaypoints;
    private GameObject[]RedBotWaypoints;

    private Vector3 redSpawnPoint1 = new Vector3(-50, 0, 6); // Red Minion spawn position
    private Vector3 blueSpawnPoint1 = new Vector3(50, 0, 6); // Blue Minion spawn position

    private Vector3 redSpawnPoint2 = new Vector3(-50, 0, -6); // Red Minion spawn position
    private Vector3 blueSpawnPoint2 = new Vector3(50, 0, -6); // Blue Minion spawn position

    private int minionsPerWave = 6; // Total minions per team in a wave

    void Start()
    {
        // red waypoints are in reverse order
        RedTopWaypoints = new GameObject[BlueTopWaypoints.Length];
        RedBotWaypoints = new GameObject[BlueBotWaypoints.Length];
        for (int i = 0; i < BlueTopWaypoints.Length; i++)
        {
            RedTopWaypoints[i] = BlueTopWaypoints[BlueTopWaypoints.Length - 1 - i];
            RedBotWaypoints[i] = BlueBotWaypoints[BlueBotWaypoints.Length - 1 - i];
        }

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
            SpawnMinion(redMinionPrefab, redSpawnPoint1, isCaster, RedTopWaypoints);

            // Spawn a Blue Minion
            SpawnMinion(blueMinionPrefab, blueSpawnPoint1, isCaster, BlueTopWaypoints);

            SpawnMinion(redMinionPrefab, redSpawnPoint2, isCaster, RedBotWaypoints);

            SpawnMinion(blueMinionPrefab, blueSpawnPoint2, isCaster, BlueBotWaypoints);


            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMinion(GameObject prefab, Vector3 spawnPosition, bool isCaster, GameObject[] waypoints)
    {
        GameObject minion = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Set the isCaster property on the spawned minion
        MinionsMovement minionScript = minion.GetComponent<MinionsMovement>();
        if (minionScript != null)
        {
            minionScript.isCaster = isCaster;
            minionScript.waypoints = waypoints;
        }
    }
}
