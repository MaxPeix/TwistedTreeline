using System.Collections;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static GameEngine Instance;

    private void Awake()
    {
        // Ensure only one instance of GameEngine exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this GameObject across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Coroutine to handle respawning
    public IEnumerator RespawnCoroutine(LifeSystem lifeSystem, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        lifeSystem.FinishRespawn();
    }
}
