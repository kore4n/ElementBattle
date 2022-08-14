using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using Steamworks;
using UnityEngine.SceneManagement;

public class FPSNetworkManager : NetworkManager
{
    public List<FPSPlayer> players = new List<FPSPlayer>();
    public List<SpectatorCameraController> spectatorCameras = new List<SpectatorCameraController> { };

    public List<GameObject> playerCharacterPrefabs;

    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private GameObject spectatorCameraPrefab;    // Camera to spawn after death

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    #region GetSets

    public GameObject GetSpectatorCamera()
    {
        return spectatorCameraPrefab;
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

    // Called on client join
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
            GameManager gameManager = Instantiate(gameManagerPrefab);

            NetworkServer.Spawn(gameManager.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        FPSPlayer player = conn.identity.GetComponent<FPSPlayer>();

        if (player.GetActiveSpectatorCamera() != null)
        {
            spectatorCameras.Remove(player.GetActiveSpectatorCamera());
        }

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public void SpawnSpectatorCamera(Vector3 spawnPosition, Quaternion spawnRotation, NetworkConnectionToClient networkConnectionToClient)
    {
        GameObject spectatorCamera = Instantiate(
                spectatorCameraPrefab,
                spawnPosition,
                spawnRotation);
        NetworkServer.Spawn(spectatorCamera, networkConnectionToClient);

        //Debug.Log("Spawned spectator camera!");

        spectatorCameras.Add(spectatorCamera.GetComponent<SpectatorCameraController>());

        FPSPlayer player = networkConnectionToClient.identity.GetComponent<FPSPlayer>();
        player.SetActiveSpectatorCamera(spectatorCamera);
    }

    #endregion

    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // Create the player
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