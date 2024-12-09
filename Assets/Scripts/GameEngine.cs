using System.Collections;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static GameEngine Instance;

    private AudioSource music;

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

        // Get the AudioSource component from the GameObject
        music = GetComponent<AudioSource>();
        music.loop = true;
        music.Play();
    }

    // Coroutine to handle respawning
    public IEnumerator RespawnCoroutine(LifeSystem lifeSystem, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        lifeSystem.FinishRespawn();
    }
}
