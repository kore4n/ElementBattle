using Game.Abilities;
using Game.Combat;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject activePlayerCharacter;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    public string playerName;

    // TODO: Make this private
    [SyncVar(hook = nameof(ClientHandlePlayerElementUpdated))]
    public Constants.Element playerElement = Constants.Element.Missing;

    // TODO: Make this private
    [SyncVar(hook = nameof(ClientHandlePlayerTeamUpdated))]
    public Constants.Team playerTeam = Constants.Team.Missing;

    [SyncVar(hook = nameof(ClientHandlePlayerReadyUpdated))]
    private bool readiedUp = false; // Logic not taken into account for spectators for game start so dont worry about it

    public Sprite elementSprite = null;

    public static Action OnPlayerSpawn;
    public static event Action ClientOnInfoUpdated; // Used for everything
    public static event Action ClientOnChooseTeam;  
    public static event Action ClientOnMeChooseElement;   // When "this player" chooses an element
    public static event Action<Constants.Team> ClientOnMeChooseTeam;  // When anyone has chosen an element
    public static event Action ClientOnAnyoneChooseElement;  // When anyone has chosen an element

    [SerializeField] private Sprite[] elementSprites = new Sprite[4];
    [SerializeField] AbilitySet[] abilitySets = new AbilitySet[4];

    #region Subscribe/Unsubscribe
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }
    #endregion

    #region Getters/Setters

    public PlayerCharacter GetActivePlayerCharacter()
    {
        return activePlayerCharacter.GetComponent<PlayerCharacter>();
    }

    public Constants.Element GetElement()
    {
        return playerElement;
    }

    [Server]
    public void SetDisplayName(string newName)
    {
        playerName = newName;
    }

    [Server]
    public void SetTeam(Constants.Team team)
    {
        playerTeam = team;
        Debug.Log("My team is being set!");
        if (team != Constants.Team.Spectator) { return; }

        FPSNetworkManager networkManager = (FPSNetworkManager)NetworkManager.singleton;
        GameManager gameManager = GameManager.singleton;

        // Make spectator camera
        GameObject spectatorCamera = Instantiate(
                networkManager.GetSpectatorCamera(),
                gameManager.GetSpectatorSpawn().position,
                Quaternion.identity);
        NetworkServer.Spawn(spectatorCamera, connectionToClient);

        networkManager.spectatorCameras.Add(spectatorCamera.GetComponent<SpectatorCameraController>());
    }

    [Server]
    public void SetReady(bool newReadyState)
    {
        readiedUp = newReadyState; 
    }

    [Command]
    public void CmdSetReadiedUp(bool newReadyState)
    {
        SetReady(newReadyState);
    }

    public bool HasActivePlayerCharacter()
    {
        if (activePlayerCharacter != null) { return true; }

        return false;
    }

    public void SetActivePlayerCharacter(GameObject newActivePlayerCharacter)
    {
        activePlayerCharacter = newActivePlayerCharacter;
    }

    public bool GetReadiedUp()
    {
        return readiedUp;
    }


    public Constants.Team GetTeam()
    {
        return playerTeam;
    }

    [Command]
    public void CmdSetTeam(Constants.Team team)
    {
        SetTeam(team);

        TargetClientTeamAvailable(team);
    }

    public Sprite GetElementSprite()
    {
        return elementSprite;
    }

    public string GetDisplayName()
    {
        return playerName;
    }

    #endregion

    #region Server

    [Command]
    public void CmdChooseElement(PlayerInfo playerInfo)
    {
        FPSNetworkManager networkManager = (FPSNetworkManager)NetworkManager.singleton;

        // Check if anyone on same team is the same element
        foreach (FPSPlayer player in networkManager.players)
        {
            if (player.GetTeam() != playerTeam) { continue; }

            if (player.playerElement == playerInfo.element) { return; }
        }

        // Element is open
        TargetClientElementAvailable();    // Tell user to close menu

        GameManager gameManager = GameManager.singleton;

        // If game in progress create spectator camera
        if (gameManager.IsGameInProgress())
        {
            GameObject spectatorCamera = Instantiate(
                networkManager.GetSpectatorCamera(),
                gameManager.GetSpectatorSpawn().position,
                Quaternion.identity);
            NetworkServer.Spawn(spectatorCamera, connectionToClient);

            networkManager.spectatorCameras.Add(spectatorCamera.GetComponent<SpectatorCameraController>());
            return; 
        }

        // Only create player if in pregame

        CreatePlayerCharacter(playerInfo);
    }

    [Server]
    private void CreatePlayerCharacter(PlayerInfo playerInfo)
    {
        if (activePlayerCharacter != null) { return; }

        Vector3 spawnLocation = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        switch (playerTeam)
        {
            case Constants.Team.Red:
                Transform redSpawn = GameManager.singleton.GetRedSpawn();
                spawnLocation = redSpawn.position;
                spawnRotation = redSpawn.rotation;
                break;
            case Constants.Team.Blue:
                Transform blueSpawn = GameManager.singleton.GetBlueSpawn();
                spawnLocation = blueSpawn.position;
                spawnRotation = blueSpawn.rotation;
                break;
            case Constants.Team.Spectator:
                // TODO: Spawn spectator camera

                break;
            case Constants.Team.Missing:
                Debug.Log("Invalid Team. Error has occurred.");
                break;
        }

        playerElement = playerInfo.element;
        GameObject myPlayer = Instantiate(((FPSNetworkManager)NetworkManager.singleton).playerCharacterPrefabs[(int)playerElement], spawnLocation, Quaternion.identity);

        // TODO: Line doesn't work. Want player to spawn facing correct side.
        myPlayer.GetComponent<MyCharacterMovement>().viewTransform.rotation = spawnRotation;

        activePlayerCharacter = myPlayer;

        PlayerCharacter playerCharacter = activePlayerCharacter.GetComponent<PlayerCharacter>();
        playerCharacter.playerCharacterName = playerName;
        playerCharacter.SetTeam(playerTeam);
        playerCharacter.SetElement(playerElement);

        myPlayer.GetComponent<PlayerCharacter>().FPSOwner = this;
        NetworkServer.Spawn(myPlayer, connectionToClient);
    }

    [Server]
    public void RespawnPlayer()
    {
        PlayerInfo playerInfo = new PlayerInfo
        { 
            element = playerElement,
            playerName = playerName,
        };

        CreatePlayerCharacter(playerInfo);
    }
    #endregion

    #region Client

    private void Start()
    {
        OnPlayerSpawn?.Invoke();
    }

    private void Update()
    {
        if (!hasAuthority) { return; }

        if (playerTeam != Constants.Team.Red && playerTeam != Constants.Team.Blue) { return; }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            CmdSetReadiedUp(!readiedUp);
        }
    }

    [TargetRpc]
    private void TargetClientElementAvailable()
    {
        ClientOnMeChooseElement?.Invoke();
    }
    
    [TargetRpc]
    private void TargetClientTeamAvailable(Constants.Team team)
    {
        ClientOnMeChooseTeam?.Invoke(team);
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }   // If running on server as well - stop

        DontDestroyOnLoad(gameObject);

        ((FPSNetworkManager)NetworkManager.singleton).players.Add(this);

    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }  // Dont let server run anything below

        ((FPSNetworkManager)NetworkManager.singleton).players.Remove(this); // Let all clients remove player list

        if (!hasAuthority) { return; }  // TODO: Find out why this line is here
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandlePlayerElementUpdated(Constants.Element oldElement, Constants.Element newElement)
    {
        elementSprite = elementSprites[(int)playerElement];

        ClientOnAnyoneChooseElement?.Invoke();
        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandlePlayerSpriteUpdated(Sprite oldSprite, Sprite newSprite)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandlePlayerTeamUpdated(Constants.Team oldTeam, Constants.Team newTeam)
    {
        ClientOnInfoUpdated?.Invoke();
        //ClientOnChooseTeam?.Invoke();
    }

    private void ClientHandlePlayerReadyUpdated(bool oldReadyState, bool newReadyState)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
