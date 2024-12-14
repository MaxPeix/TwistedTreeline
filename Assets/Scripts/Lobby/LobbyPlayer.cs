using Unity.Netcode;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    private ulong clientId;
    private int currentLobbyId;

    public void SetClientId(ulong id)
    {
        clientId = id;
    }

    public ulong GetClientId()
    {
        return clientId;
    }

    public void ChooseTeam(string teamName)
    {
        if (IsOwner)
        {
            GameManager.Instance.AssignTeamToPlayer(currentLobbyId, clientId, teamName);
        }
    }

    public int GetCurrentLobbyId()
    {
        return currentLobbyId;
    }

    public void JoinLobby(int lobbyId)
    {
        currentLobbyId = lobbyId;
    }
}
