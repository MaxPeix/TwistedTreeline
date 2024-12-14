using System.Collections.Generic;

public class Lobby
{
    public int LobbyId { get; private set; }
    public string LobbyName { get; private set; }
    public List<ulong> Players { get; private set; } = new List<ulong>();

    public Lobby(int lobbyId, string lobbyName)
    {
        LobbyId = lobbyId;
        LobbyName = lobbyName;
    }

    public void AddPlayer(ulong playerId)
    {
        if (!Players.Contains(playerId))
        {
            Players.Add(playerId);
        }
    }

    public void RemovePlayer(ulong playerId)
    {
        if (Players.Contains(playerId))
        {
            Players.Remove(playerId);
        }
    }
}
