using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerCharacter;
    private GameObject activePlayerCharacter;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    public string playerName;

    [SyncVar(hook = nameof(ClientHandlePlayerElementUpdated))]
    public Constants.Element playerElement = Constants.Element.Missing;

    [SyncVar(hook = nameof(ClientHandlePlayerTeamUpdated))]
    public Constants.Team playerTeam = Constants.Team.Missing;

    [SyncVar]
    private bool readiedUp = false;

    //Remove later
    //[SyncVar(hook = nameof(ClientHandlePlayerSpriteUpdated))]
    //[SyncVar]
    public Sprite elementSprite = null;

    public static Action OnPlayerSpawn;
    public static event Action ClientOnInfoUpdated;

    [SerializeField] private Sprite[] elementSprites = new Sprite[4];

    [Server]
    public void SetDisplayName(string newName)
    {
        playerName = newName;
    }

    [Server]    //TODO: Should this be server?
    public void SetTeam(Constants.Team team)
    {
        playerTeam = team;

        FPSNetworkManager myNetworkManager = ((FPSNetworkManager)NetworkManager.singleton);

        
    }

    public bool GetReadiedUp()
    {
        return readiedUp;
    }

    [Command]
    public void CmdSetReadiedUp(bool newReadyState)
    {
        readiedUp = newReadyState;

        RpcClientInfoUpdated();
    }

    public Constants.Team GetTeam()
    {
        return playerTeam;
    }

    [Command]
    public void CmdSetTeam(Constants.Team team)
    {
        SetTeam(team);
    }

    public Sprite GetElementSprite()
    {
        return elementSprite;
    }

    public string GetDisplayName()
    {
        return playerName;
    }

    #region Server

    [Command]
    public void CmdCreatePlayerCharacter(PlayerInfo playerInfo)
    {
        CreatePlayerCharacter(playerInfo);
    }

    [Server]
    private void CreatePlayerCharacter(PlayerInfo playerInfo)
    {
        if (activePlayerCharacter != null) { return; }

        GameObject myPlayer = Instantiate(playerCharacter, new Vector3(0, 0, 0), Quaternion.identity);
        activePlayerCharacter = myPlayer;
        //SetDisplayName(playerInfo.playerName);

        playerElement = playerInfo.element;
        activePlayerCharacter.GetComponent<PlayerCharacter>().SetTeam(playerTeam);

        NetworkServer.Spawn(myPlayer, connectionToClient);
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

        if (Input.GetKeyDown(KeyCode.F4))
        {
            CmdSetReadiedUp(!readiedUp);
        }
    }

    [ClientRpc]
    private void RpcClientInfoUpdated()
    {
        ClientOnInfoUpdated?.Invoke();
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

        if (!hasAuthority) { return; }
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandlePlayerElementUpdated(Constants.Element oldElement, Constants.Element newElement)
    {
        elementSprite = elementSprites[(int)playerElement];

        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandlePlayerSpriteUpdated(Sprite oldSprite, Sprite newSprite)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void ClientHandlePlayerTeamUpdated(Constants.Team oldTeam, Constants.Team newTeam)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
