using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;

public class FPSNetworkManager : NetworkManager
{
    public List<FPSPlayer> players = new List<FPSPlayer> ();

    public int redPlayers = 0;
    public int bluePlayers = 0;
    public int specPlayers = 0;

    [SerializeField] private GameObject spectatorCamera;    // Camera to spawn after death

    [SerializeField] private TMP_InputField playerNameInputField = null;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public GameObject GetSpectatorCamera()
    {
        return spectatorCamera;
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateFPSPlayerMessage>(OnCreatePlayer);
    }

    private void OnCreatePlayer(NetworkConnectionToClient conn, CreateFPSPlayerMessage message)
    {
        GameObject playerGameobject = Instantiate(playerPrefab);

        FPSPlayer player = playerGameobject.GetComponent<FPSPlayer>();
        player.SetDisplayName(message.playerName);

        NetworkServer.AddPlayerForConnection(conn, playerGameobject);

        players.Add(player);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        //FPSPlayer player = conn.identity.GetComponent<FPSPlayer>();

        //Debug.Log("Adding player to list!");
        //players.Add(player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        FPSPlayer player = conn.identity.GetComponent<FPSPlayer>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    #endregion

    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        CreateFPSPlayerMessage createFPSPlayerMessage = new CreateFPSPlayerMessage()
        {
            playerName = playerNameInputField.text
        };
        NetworkClient.Send(createFPSPlayerMessage);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }

    #endregion
}