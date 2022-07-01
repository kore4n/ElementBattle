using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField] private Health health;

    public static Action<int> ServerOnPlayerDie;
    public static event Action<PlayerCharacter> ServerOnPlayerSpawned;
    public static event Action<PlayerCharacter> ServerOnPlayerDespawned;

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;

        ServerOnPlayerSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {

        ServerOnPlayerDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

}
