using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;

    public static GameManager singleton
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static event Action OnGameManagerSpawn;
    public static event Action ClientOnGameStart;
    public static event Action<Constants.Team> ClientOnGameOver;

    public static event Action ClientOnRoundWin;
    public static event Action<LoweringArena> ServerOnLowerArena;
    public static event Action<Constants.GameAction> ServerOnArenaAction;

    [SerializeField] private LoweringArena[] platforms = new LoweringArena[4];   // Platforms that will lower

    private List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

    private int redCharacters = 0;
    private int blueCharacters = 0;
    private int specCharacters = 0;

    [SyncVar(hook = nameof(UpdateRounds))]
    public int blueRounds = 0;
    [SyncVar(hook = nameof(UpdateRounds))]
    public int redRounds = 0;
    [SerializeField]
    private int winLimit = 3;

    [SyncVar]
    [SerializeField] 
    private bool isGameInProgress = false; // TODO: Remove SerializeField later, just to see


    [SerializeField] public List<Transform> redSpawnPositions = new List<Transform>();
    [SerializeField] public List<Transform> blueSpawnPositions = new List<Transform>();
    [SerializeField] public List<Transform> spectatorSpawnPositions = new List<Transform>();

    //[SerializeField] public readonly SyncList<Transform> redSpawnPositions = new SyncList<Transform>();
    //[SerializeField] public readonly SyncList<Transform> blueSpawnPositions = new SyncList<Transform>();
    //[SerializeField] public readonly SyncList<Transform> spectatorSpawnPositions = new SyncList<Transform>();

    #region Getters/Setters
    public Transform GetRedSpawn()
    {
        return redSpawnPositions[0];
    }

    public Transform GetBlueSpawn()
    {
        return blueSpawnPositions[0];
    }

    public Transform GetSpectatorSpawn()
    {
        return spectatorSpawnPositions[0];
    }

    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }
    #endregion

    #region Server

    private void Start()
    {
        //Debug.Log("Spawning game manager");
        OnGameManagerSpawn?.Invoke();

        //if (!isServer) { return; }

        // Only server manages spawns
        PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

        // Add spawnpoints to manager to use to spawn character
        foreach (PlayerSpawnPoint spawnPoint in spawnPoints)
        {
            switch (spawnPoint.GetTeam())
            {
                case Constants.Team.Red:
                    redSpawnPositions.Add(spawnPoint.transform);
                    break;
                case Constants.Team.Blue:
                    blueSpawnPositions.Add(spawnPoint.transform);
                    break;
                case Constants.Team.Spectator:
                    spectatorSpawnPositions.Add(spawnPoint.transform);
                    break;
                case Constants.Team.Missing:
                    Debug.Log("Invalid Team. Error has occurred.");
                    break;
            }
        }
    }

    public override void OnStartServer()
    {
        PlayerCharacter.ServerOnPlayerCharacterSpawned += ServerHandlePlayerCharacterSpawned;
        PlayerCharacter.ServerOnPlayerCharacterDespawned += ServerHandlePlayerCharacterDespawned;
        FPSPlayer.ClientOnInfoUpdated += ServerHandleClientInfoUpdated;
    }

    public override void OnStopServer()
    {
        PlayerCharacter.ServerOnPlayerCharacterSpawned -= ServerHandlePlayerCharacterSpawned;
        PlayerCharacter.ServerOnPlayerCharacterDespawned -= ServerHandlePlayerCharacterDespawned;
        FPSPlayer.ClientOnInfoUpdated -= ServerHandleClientInfoUpdated;
    }

    [Server]
    private void ServerHandlePlayerCharacterSpawned(PlayerCharacter playerCharacter)
    {
        playerCharacters.Add(playerCharacter);

        switch (playerCharacter.GetTeam())
        {
            case Constants.Team.Red:
                redCharacters++;
                break;
            case Constants.Team.Blue:
                blueCharacters++;
                break;
            case Constants.Team.Spectator:
                specCharacters++;
                break;
            case Constants.Team.Missing:
                Debug.Log("Invalid Team. Error has occurred.");
                break;
        }
    }

    [Server]
    private void ServerHandlePlayerCharacterDespawned(PlayerCharacter playerCharacter)
    {
        playerCharacters.Remove(playerCharacter);

        switch (playerCharacter.GetTeam())
        {
            case Constants.Team.Red:
                redCharacters--;
                break;
            case Constants.Team.Blue:
                blueCharacters--;
                break;
            case Constants.Team.Spectator:
                specCharacters--;
                break;
            case Constants.Team.Missing:
                Debug.Log("Invalid Team. Error has occurred.");
                break;
        }

        if (!IsGameInProgress()) { return; }

        // Game is in progress! Update rounds

        if (blueCharacters == 0)
        {
            ServerOnUpdateRounds(Constants.Team.Red);
        }
        else if (redCharacters == 0)
        {
            ServerOnUpdateRounds(Constants.Team.Blue);
        }

        if (redRounds == winLimit)
        {
            RpcGameOver(Constants.Team.Red);
            EndGame();
        }
        else if (blueRounds == winLimit)
        {
            RpcGameOver(Constants.Team.Blue);
            EndGame();
        }

        Invoke(nameof(RestartRound), Constants.timeAfterRoundEnd);

        int roundSum = redRounds + blueRounds;

        if (roundSum == 0)
        { 
            Debug.Log("Game over!");
            return;
        }

        Debug.Log($"Round {redRounds + blueRounds} over! ");
    }

    private void EndGame()
    {
        isGameInProgress = false;
        blueRounds = 0;
        redRounds = 0;

        foreach (FPSPlayer player in ((FPSNetworkManager)NetworkManager.singleton).players)
        {
            player.SetReady(false);
        }
    }

    [Server]
    private void ServerHandleClientInfoUpdated()
    {
        if (isGameInProgress) { return; }

        int reds = 0;
        int blues = 0;

        List<FPSPlayer> players = ((FPSNetworkManager)NetworkManager.singleton).players;
        foreach (var player in players)
        {
            if (!player.GetReadiedUp()) { return; }

            switch (player.GetTeam())
            {
                case Constants.Team.Red:
                    reds++;
                    break;
                case Constants.Team.Blue:
                    blues++;
                    break;
            }
        }

        // All players are ready

        if (reds < 1 || blues < 1) { return; }

        // All players are ready and there's at least one person on each team

        isGameInProgress = true;

        Invoke(nameof(RestartRound), Constants.timeAfterRoundEnd);

        StartCoroutine(CoroutineRpcOnClientArenaAction(Constants.GameAction.Start));
    }

    IEnumerator CoroutineRpcOnClientArenaAction(Constants.GameAction action)
    {
        yield return new WaitForSeconds(Constants.timeAfterRoundEnd);

        RpcOnClientArenaAction(action);

    }

    private void ServerOnUpdateRounds(Constants.Team team)
    {
        // Automatically updates round wins + UI with syncvars and hooks
        switch (team)
        {
            case Constants.Team.Red:
                redRounds++;
                break;
            case Constants.Team.Blue:
                blueRounds++;
                break;
            case Constants.Team.Missing:
                Debug.Log("Invalid Team. Error has occurred.");
                break;
        }
    }

    private void RestartRound()
    {
        List<FPSPlayer> players = ((FPSNetworkManager)NetworkManager.singleton).players;
        List<SpectatorCameraController> spectatorCameras = ((FPSNetworkManager)NetworkManager.singleton).spectatorCameras;

        // Respawn all players
        foreach (FPSPlayer player in players)
        {
            if (player.HasActivePlayerCharacter()) { continue; }

            player.RespawnPlayer();
        }

        foreach (FPSPlayer player in players)
        {
            // Player is respawned, now move player characters to the correct position
            MoveToCorrectSpot(player.GetActivePlayerCharacter());
        }

        // Destroy all spectator cameras
        foreach (SpectatorCameraController spectatorCamera in spectatorCameras)
        {
            //spectatorCamera.netIdentity.serverOnly = true;
            NetworkServer.Destroy(spectatorCamera.gameObject);
        }

        foreach (FPSPlayer player in players)
        {
            player.SetActiveSpectatorCamera(null);
        }

        spectatorCameras.Clear();
    }

    [Server]
    private void MoveToCorrectSpot(PlayerCharacter playerCharacter)
    {
        var nt = playerCharacter.GetComponent<NetworkTransform>();

        switch (playerCharacter.GetTeam())
        {
            case (Constants.Team.Red):
                nt.RpcTeleport(redSpawnPositions[0].position);
                break;
            case (Constants.Team.Blue):
                nt.RpcTeleport(blueSpawnPositions[0].position);
                break;
            case (Constants.Team.Spectator):
                Debug.Log("This should not exist! Spectators should not be allowed to own player characters.");
                break;
            case (Constants.Team.Missing):
                Debug.Log("This should not exist! Player character team is missing. Player characters should either be blue or red.");
                break;
        }
    }

    #endregion

    #region Clients

    private void UpdateRounds(int oldRounds, int newRounds)
    {
        ClientOnRoundWin?.Invoke();
    }

    [ClientRpc]
    private void RpcGameOver(Constants.Team winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    [ClientRpc]
    private void RpcOnClientArenaAction(Constants.GameAction gameAction)
    {
        if (gameAction == Constants.GameAction.Start) { ClientOnGameStart?.Invoke(); }

        Debug.Log("Game is starting!");
    }

    #endregion
}
