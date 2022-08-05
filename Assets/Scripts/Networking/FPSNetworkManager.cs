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
    public List<GameObject> playerCharacterPrefabs;

    //private bool isGameInProgress = false;

    [SerializeField] private GameObject spectatorCameraPrefab;    // Camera to spawn after death

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    [SerializeField] private List<Transform> redSpawnPositions = new List<Transform>();
    [SerializeField] private List<Transform> blueSpawnPositions = new List<Transform>();

    #region GetSets

    public GameObject GetSpectatorCamera()
    {
        return spectatorCameraPrefab;
    }

    public Transform GetRedSpawn()
    {
        return redSpawnPositions[0];
    }

    public Transform GetBlueSpawn()
    {
        return blueSpawnPositions[0];
    }

    //public bool IsGameInProgress()
    //{
    //    return isGameInProgress;
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

    //public override void OnServerSceneChanged(string sceneName)
    //{
    //    if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
    //    {
    //        GameObject gameOverHandler = Instantiate(gameOverHandlerPrefab);

    //        NetworkServer.Spawn(gameOverHandler);
    //    }
    //}

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