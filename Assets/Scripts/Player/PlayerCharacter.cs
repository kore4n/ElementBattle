using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    // TODO: Make serializefield later
    public string playerCharacterName = "Missing name";
    [SerializeField] private Health health;

    [SyncVar] 
    private Constants.Team team = Constants.Team.Missing;

    [SyncVar] private Constants.Element element = Constants.Element.Missing;

    public static Action<int> ServerOnPlayerCharacterDie;
    public static event Action<PlayerCharacter> ServerOnPlayerCharacterSpawned;
    public static event Action<PlayerCharacter> ServerOnPlayerCharacterDespawned;

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
        ServerOnPlayerCharacterDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }
}
