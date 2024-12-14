using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private LobbyManager lobbyManager;

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

        lobbyManager = LobbyManager.Instance;
    }

    public void CreateLobby(string lobbyName)
    {
        Lobby newLobby = lobbyManager.CreateLobby(lobbyName);
        Debug.Log($"Lobby '{newLobby.LobbyName}' created with ID: {newLobby.LobbyId}");

        // ajouter le player au lobby ?
    }

    private void UpdateLobbyUI(int lobbyId)
    {
        UIManager.Instance.UpdateLobbyPlayerList(lobbyId);
    }


    public void JoinLobby(int lobbyId, ulong playerId)
    {
        Lobby lobby = LobbyManager.Instance.GetLobbyById(lobbyId);
        if (lobby != null)
        {
            lobby.AddPlayer(playerId);
            Debug.Log($"Player {playerId} joined lobby {lobby.LobbyName}.");
            UpdateLobbyUI(lobbyId); // Actualisez l'interface utilisateur
        }
        else
        {
            Debug.LogError($"Lobby with ID {lobbyId} not found.");
        }
    }

    public void LeaveLobby(int lobbyId, ulong playerId)
    {
        Lobby lobby = LobbyManager.Instance.GetLobbyById(lobbyId);
        if (lobby != null)
        {
            lobby.RemovePlayer(playerId);
            Debug.Log($"Player {playerId} left lobby {lobby.LobbyName}.");
            UpdateLobbyUI(lobbyId); // Actualisez l'interface utilisateur
        }
    }


    public void AssignTeamToPlayer(int lobbyId, ulong clientId, string teamName)
    {
        Lobby lobby = lobbyManager.GetLobbyById(lobbyId);
        if (lobby != null)
        {
            Debug.Log($"Player {clientId} assigned to team '{teamName}' in lobby '{lobby.LobbyName}'.");
        }
    }
}
