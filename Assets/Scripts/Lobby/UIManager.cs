using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Netcode;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button refreshLobbyListButton;
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private GameObject lobbyListItemPrefab;

    [SerializeField] private Button redTeamButton;
    [SerializeField] private Button blueTeamButton;

    private int currentLobbyId;

    [SerializeField] private Transform playerListContainer; // Conteneur pour la liste des joueurs
    [SerializeField] private GameObject playerListItemPrefab; // Prefab pour afficher un joueur

    public void UpdateLobbyPlayerList(int lobbyId)
    {
        // Nettoyez la liste existante
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        // Récupérez les joueurs du lobby
        Lobby lobby = LobbyManager.Instance.GetLobbyById(lobbyId);
        if (lobby == null)
        {
            Debug.LogError($"Lobby with ID {lobbyId} not found.");
            return;
        }

        // Créez un item pour chaque joueur
        foreach (ulong playerId in lobby.Players)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab, playerListContainer);
            TextMeshProUGUI playerNameText = playerItem.GetComponentInChildren<TextMeshProUGUI>();
            if (playerNameText != null)
            {
                playerNameText.text = $"Player {playerId}";
            }
        }
    }

    private void Start()
    {
        createLobbyButton.onClick.AddListener(CreateLobby);
        refreshLobbyListButton.onClick.AddListener(RefreshLobbyList);

        redTeamButton.onClick.AddListener(() => ChooseTeam("Red"));
        blueTeamButton.onClick.AddListener(() => ChooseTeam("Blue"));
    }

    private void CreateLobby()
    {
        string lobbyName = "Lobby " + (LobbyManager.Instance.GetLobbies().Count + 1);
        GameManager.Instance.CreateLobby(lobbyName);
    }

    private void RefreshLobbyList()
    {
        if (LobbyManager.Instance == null)
        {
            Debug.LogError("LobbyManager.Instance is null! Make sure it is in the scene.");
            return;
        }

        if (lobbyListContainer == null)
        {
            Debug.LogError("lobbyListContainer is null! Assign it in the inspector.");
            return;
        }

        foreach (Transform child in lobbyListContainer)
        {
            Destroy(child.gameObject);
        }

        if (lobbyListItemPrefab == null)
        {
            Debug.LogError("lobbyListItemPrefab is null! Assign it in the inspector.");
            return;
        }

        var lobbies = LobbyManager.Instance.GetLobbies();
        foreach (var lobby in lobbies)
        {
            GameObject lobbyItem = Instantiate(lobbyListItemPrefab, lobbyListContainer);
            TextMeshProUGUI lobbyNameText = lobbyItem.GetComponentInChildren<TextMeshProUGUI>();
            Button joinButton = lobbyItem.GetComponentInChildren<Button>();

            lobbyNameText.text = $"{lobby.LobbyName} (ID: {lobby.LobbyId})";
            joinButton.onClick.AddListener(() => JoinLobby(lobby.LobbyId));
        }
    }


    private void JoinLobby(int lobbyId)
    {
        GameManager.Instance.JoinLobby(lobbyId, NetworkManager.Singleton.LocalClientId);
        currentLobbyId = lobbyId;
    }

    private void ChooseTeam(string teamName)
    {
        var localPlayer = FindObjectsOfType<LobbyPlayer>().FirstOrDefault(p => p.IsOwner);
        if (localPlayer != null)
        {
            localPlayer.ChooseTeam(teamName);
        }
    }
}
