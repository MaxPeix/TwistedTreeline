using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    private List<Lobby> lobbies = new List<Lobby>();
    private int lobbyIdCounter = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Lobby CreateLobby(string lobbyName)
    {
        int newLobbyId = lobbyIdCounter++;
        Lobby newLobby = new Lobby(newLobbyId, lobbyName);
        lobbies.Add(newLobby);
        return newLobby;
    }

    public List<Lobby> GetLobbies()
    {
        return lobbies;
    }

    public Lobby GetLobbyById(int lobbyId)
    {
        return lobbies.Find(lobby => lobby.LobbyId == lobbyId);
    }
}
