using Game.Abilities;
using Game.Combat;
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

    [SyncVar(hook = nameof(ClientHandlePlayerReadyUpdated))]
    public bool readiedUp = false;  // Remove Public later

    //Remove later
    //[SyncVar(hook = nameof(ClientHandlePlayerSpriteUpdated))]
    //[SyncVar]
    public Sprite elementSprite = null;

    public static Action OnPlayerSpawn;
    public static event Action ClientOnInfoUpdated;

    [SerializeField] private Sprite[] elementSprites = new Sprite[4];
    [SerializeField] AbilitySet[] abilitySets = new AbilitySet[4];

    #region Getters/Setters

    [Server]
    public void SetDisplayName(string newName)
    {
        playerName = newName;
    }

    [Server]
    public void SetTeam(Constants.Team team)
    {
        playerTeam = team;
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
    public void CmdCreatePlayerCharacter(PlayerInfo playerInfo)
    {
        CreatePlayerCharacter(playerInfo);
    }

    [Server]
    private void CreatePlayerCharacter(PlayerInfo playerInfo)
    {
        if (activePlayerCharacter != null) { return; }

        GameObject myPlayer = Instantiate(this.playerCharacter, new Vector3(0, 0, 0), Quaternion.identity);
        activePlayerCharacter = myPlayer;
        
        playerElement = playerInfo.element;
        PlayerCharacter playerCharacter = activePlayerCharacter.GetComponent<PlayerCharacter>();
        playerCharacter.playerCharacterName = playerName;
        playerCharacter.SetTeam(playerTeam);
        playerCharacter.SetElement(playerElement);

        //activePlayerCharacter.GetComponent<Combatant>().abilitySet = abilitySets[(int)playerElement];

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

    private void ClientHandlePlayerReadyUpdated(bool oldReadyState, bool newReadyState)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
