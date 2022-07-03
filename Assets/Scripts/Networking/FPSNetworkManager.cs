using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;
using Steamworks;
using UnityEngine.SceneManagement;

public class FPSNetworkManager : NetworkManager
{
    public List<FPSPlayer> players = new List<FPSPlayer>();

    private int redPlayers = 0;
    private int bluePlayers = 0;
    private int specPlayers = 0;

    private bool isGameInProgress = false;

    [SerializeField] private GameObject spectatorCameraPrefab;    // Camera to spawn after death
    [SerializeField] private GameObject gameOverHandlerPrefab;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    #region GetSets

    public GameObject GetSpectatorCamera()
    {
        return spectatorCameraPrefab;
    }

    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }

    //public int GetRedPlayers()
    //{
    //    return redPlayers;
    //}

    //public int GetBluePlayers()
    //{
    //    return bluePlayers;
    //}

    //public int GetSpecPlayers()
    //{
    //    return specPlayers;
    //}

    //public void SetRedPlayers(int newRedPlayers)
    //{
    //    redPlayers = newRedPlayers;
    //}

    //public void SetBluePlayers(int newBluePlayers)
    //{
    //    bluePlayers = newBluePlayers;
    //}

    //public void SetSpecPlayers(int newSpecPlayers)
    //{
    //    specPlayers = newSpecPlayers;
    //}

    #endregion

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

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameObject gameOverHandler = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandler);
        }
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
            playerName = SteamClient.Name,
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