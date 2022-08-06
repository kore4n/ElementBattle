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

    [SerializeField] private List<Transform> redSpawnPositions = new List<Transform>();
    [SerializeField] private List<Transform> blueSpawnPositions = new List<Transform>();

    public Transform GetRedSpawn()
    {
        return redSpawnPositions[0];
    }

    public Transform GetBlueSpawn()
    {
        return blueSpawnPositions[0];
    }

    [SyncVar(hook = nameof(UpdateRounds))]
    public int blueRounds = 0;
    [SyncVar(hook = nameof(UpdateRounds))]
    public int redRounds = 0;
    [SerializeField]
    private int winLimit = 3;

    private bool isGameInProgress = false;

    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }

    #region Server

    [Server]
    private void Start()
    {
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
                    // TODO: Spawn spectator camera here spectator

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

        RestartRound();

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

        RpcOnClientArenaAction(Constants.GameAction.Start);
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

            // Respawn their player
            player.RespawnPlayer();
        }

        // Destroy all spectator cameras
        foreach (SpectatorCameraController spectatorCamera in spectatorCameras)
        {
            //spectatorCamera.netIdentity.serverOnly = true;
            NetworkServer.Destroy(spectatorCamera.gameObject);
        }

        spectatorCameras.Clear();
    }


    #endregion

    #region Client
    
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
