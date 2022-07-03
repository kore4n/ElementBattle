using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;

    public static event Action<Constants.Team> ClientOnGameOver;

    private List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

    private int redCharacters = 0;
    private int blueCharacters = 0;
    private int specCharacters = 0;

    #region Server

    public override void OnStartServer()
    {
        PlayerCharacter.ServerOnPlayerCharacterSpawned += ServerHandlePlayerCharacterSpawned;
        PlayerCharacter.ServerOnPlayerCharacterDespawned += ServerHandlePlayerCharacterDespawned;
    }

    public override void OnStopServer()
    {
        PlayerCharacter.ServerOnPlayerCharacterSpawned -= ServerHandlePlayerCharacterSpawned;
        PlayerCharacter.ServerOnPlayerCharacterDespawned -= ServerHandlePlayerCharacterDespawned;
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

        //TODO Add ! back to berginning
        if (((FPSNetworkManager)NetworkManager.singleton).IsGameInProgress()) { return; }

        Debug.Log($"Red: {redCharacters}");
        Debug.Log($"Blue: {blueCharacters}");

        if (blueCharacters == 0)
        {
            RpcGameOver(Constants.Team.Red);
        }
        else
        {
            RpcGameOver(Constants.Team.Blue);
        }

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(Constants.Team winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
