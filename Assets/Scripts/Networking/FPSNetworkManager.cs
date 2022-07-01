using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class FPSNetworkManager : NetworkManager
{
    public List<FPSPlayer> players = new List<FPSPlayer> ();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    // All element choose buttons
    public Button[] buttons;

    #region Server


    #endregion

    #region Unity Callbacks

    public override void OnValidate()
    {
        base.OnValidate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // Subscribe to all element-select buttons + in PlayerUI to tell what to spawn player as
        SubscribeElementButtons();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// This causes the server to switch scenes and sets the networkSceneName.
    /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
    /// </summary>
    /// <param name="newSceneName"></param>
    public override void ServerChangeScene(string newSceneName)
    {
        // Do specific round/game logic here
            
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    public override void OnServerChangeScene(string newSceneName) { }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">The name of the new scene.</param>
    public override void OnServerSceneChanged(string sceneName) { }

    /// <summary>
    /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    /// <param name="sceneOperation">Scene operation that's about to happen</param>
    /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    /// <summary>
    /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    /// </summary>
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }

    #endregion

    #region Server System Callbacks

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        FPSPlayer player = conn.identity.GetComponent<FPSPlayer>();

        players.Add(player);

        player.SetDisplayName($"Player {players.Count}");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        FPSPlayer player = conn.identity.GetComponent<FPSPlayer>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }


    #endregion

    public override void OnStartServer()
    {
        base.OnStartServer();

        // What happens when we receive a message?
        // Messages from "CreatePlayerMessage" will call function "OnCreatePlayer"
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);

        // Change to main map
    }

    /// <summary>
    /// Occurs upon button click. Create the player. Called when class and team are chosen.
    /// </summary>
    public void CreatePlayer(CreatePlayerMessage info)
    {
        // you can send the message here, or wherever else you want
        CreatePlayerMessage playerMessage = new CreatePlayerMessage
        {
            name = "Joe Gaba Gaba",
            element = info.element,
        };

        NetworkClient.Send(playerMessage);
    }

    /// <summary>
    /// What happens when the message is received? (This message should be received when we select a character)
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="message"></param>
    void OnCreatePlayer(NetworkConnectionToClient conn, CreatePlayerMessage message)
    {
        //// playerPrefab is the one assigned in the inspector in Network
        //// Manager but you can use different prefabs per race for example
        //GameObject gameobject = Instantiate(playerPrefab);

        //// Apply data from the message however appropriate for your game
        //// Typically Player would be a component you write with syncvars or properties
        //PlayerBase player = gameobject.GetComponent(typeof(PlayerBase)) as PlayerBase; //PlayerBase player = gameObject.GetComponent<PlayerBase>(); doesn't work for some reason

        //// Set Player Syncvars here
        //player.playerName = message.name;
        //player.element = message.element;

        //// call this to use this gameobject as the primary controller
        //NetworkServer.AddPlayerForConnection(conn, gameobject);     // Will be put in ready state

        //Debug.Log("Created player!");
    }

    public void CreatePlayerCharacter()
    {
        GameObject gameobject = Instantiate(playerPrefab);

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        PlayerBase player = gameobject.GetComponent(typeof(PlayerBase)) as PlayerBase; //PlayerBase player = gameObject.GetComponent<PlayerBase>(); doesn't work for some reason


        // call this to use this gameobject as the primary controller
        //NetworkServer.Spawn(player, conn);

        Debug.Log("Created player!");
    }

    // Called when any element button is pressed
    public void SubscribeToButton(Button button, string name, Constants.Element element)
    {
        CreatePlayerMessage playerInfo = new CreatePlayerMessage();
        playerInfo.name = name;
        playerInfo.element = element;

        button.onClick.AddListener(delegate { CreatePlayer(playerInfo); });
    }

    // Subscribe to all element-select buttons + in PlayerUI to tell what to spawn player as
    private void SubscribeElementButtons()
    {
        //SubscribeToButton(buttons[0], "fred", Constants.Element.water);
        //SubscribeToButton(buttons[1], "fred", Constants.Element.earth);
        //SubscribeToButton(buttons[2], "fred", Constants.Element.fire);
        //SubscribeToButton(buttons[3], "fred", Constants.Element.air);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }
}


public struct CreatePlayerMessage : NetworkMessage
{
    public string name;

    public Constants.Element element;
}