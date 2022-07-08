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

    public static event Action ServerOnGameStart;
    public static event Action ServerOnGameOver;

    public static event Action ClientOnRoundWin;
    public static event Action<Constants.Team> ClientOnGameOver;
    public static event Action<LoweringArena> ServerOnLowerArena;
    public static event Action<Constants.GameAction> ServerOnArenaAction;

    [SerializeField] private LoweringArena[] platforms = new LoweringArena[4];   // Platforms that will lower

    private List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

    private int redCharacters = 0;
    private int blueCharacters = 0;
    private int specCharacters = 0;

    [SyncVar(hook = nameof(UpdateRounds))]    // Remove later
    //[SyncVar]
    public int blueRounds = 0;
    [SyncVar(hook = nameof(UpdateRounds))]     // Remove later
    public int redRounds = 0;
    [SerializeField]
    private int winLimit = 3;

    private bool isGameInProgress = false;

    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }

    #region Server

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

        // TODO: Add ! back to berginning. Should only do gameover calculations when game is in progress
        //if (((FPSNetworkManager)NetworkManager.singleton).IsGameInProgress()) { return; }
        if (!isGameInProgress)  // Pregame: Revive player and do nothing
        {
            // TODO: This is not working. Also do not spawn spectator camera
            NetworkServer.Spawn(playerCharacter.gameObject);
            return; 
        }

        //Debug.Log($"Red: {redCharacters}");
        //Debug.Log($"Blue: {blueCharacters}");

        if (blueCharacters == 0)
        {
            ServerOnUpdateRounds(Constants.Team.Red);
            return;
        }
        else if (redCharacters == 0)
        {
            ServerOnUpdateRounds(Constants.Team.Blue);
            return;
        }

        if (redRounds == winLimit)
        {
            RpcGameOver(Constants.Team.Red);
            ServerOnGameOver?.Invoke();
        }
        else
        {
            RpcGameOver(Constants.Team.Blue);
            ServerOnGameOver?.Invoke();
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

        //ServerOnArenaAction?.Invoke(Constants.GameAction.LowerArena);
        ServerOnLowerArena?.Invoke(platforms[0]);
        ServerOnLowerArena?.Invoke(platforms[1]);

        RpcOnClientArenaAction(Constants.GameAction.Start);
    }

    private void ServerOnUpdateRounds(Constants.Team team)
    {
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

        // TODO: Respawn player
        // TODO: Update player rounds UI

        RestartRound();
        //RpcClientOnRoundWin(team);

        Debug.Log($"Round {redRounds + blueRounds} over! ");
    }

    // Respawn all players
    private void RestartRound()
    {

    }


    #endregion

    #region Client

    //[ClientRpc]
    //private void RpcClientOnRoundWin(Constants.Team team)
    //{
    //    ClientOnRoundWin?.Invoke(team);
    //}

    [ClientRpc]
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
        if (gameAction == Constants.GameAction.Start) { ServerOnGameStart?.Invoke(); }

        Debug.Log("Game is starting!");
    }

    #endregion
}
