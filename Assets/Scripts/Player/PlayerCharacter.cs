using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    public FPSPlayer FPSOwner;
    // TODO: Make serializefield later
    public string playerCharacterName = "Missing name";
    [SerializeField] private Health health;

    [SyncVar] 
    private Constants.Team team = Constants.Team.Missing;

    [SyncVar] private Constants.Element element = Constants.Element.Missing;

    public static event Action<PlayerCharacter> ClientOnMyPlayerCharacterSpawned;
    public static event Action<PlayerCharacter> ClientOnMyPlayerCharacterDespawned;

    public static Action<int> ServerOnPlayerCharacterDie;
    public static event Action<PlayerCharacter> ServerOnPlayerCharacterSpawned;
    public static event Action<PlayerCharacter> ServerOnPlayerCharacterDespawned;

    private void Start()
    {
        if (!hasAuthority) { return; }


        ClientOnMyPlayerCharacterSpawned?.Invoke(this);
    }

    //public override void OnStopAuthority()
    //{
    //    ClientOnMyPlayerCharacterDespawned?.Invoke(this);
    //}

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }

        ClientOnMyPlayerCharacterDespawned?.Invoke(this);

        //Debug.Log(hasAuthority);
    }

    #region Getters/Setters
    public Constants.Team GetTeam()
    {
        return team;
    }

    public void SetTeam(Constants.Team newTeam)
    {
        team = newTeam;
    }

    public Constants.Element GetElement()
    {
        return element;
    }
    public void SetElement(Constants.Element newElement)
    {
        element = newElement;
    }
    #endregion

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;

        ServerOnPlayerCharacterSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnPlayerCharacterDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerCharacterDie?.Invoke(connectionToClient.connectionId);    // Destroy all player owned objects with health

        // What do we actually want to do when we die
        FPSPlayer fpsPlayer = connectionToClient.identity.GetComponent<FPSPlayer>();
        fpsPlayer.SetActivePlayerCharacter(null);   // Helps for logic when respawning (null = no active playerCharacter)

        // We're in midgame

        // So that players are removed from clients
        // Not sure why this fixes it - maybe because players have authority over themselves
        netIdentity.serverOnly = true;

        // Destroying player character
        NetworkServer.Destroy(gameObject);

        if (GameManager.singleton.IsGameInProgress()) { return; }

        // We're in pregame

        // Respawning player character 
        fpsPlayer.RespawnPlayer();  // If game is not in progress, it's pregame and respawn character

    }
}
