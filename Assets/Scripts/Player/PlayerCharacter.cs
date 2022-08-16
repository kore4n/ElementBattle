using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    //public FPSPlayer FPSOwner;
    // TODO: Make serializefield later
    public string playerCharacterName = "Missing name";
    [SerializeField] private Health health;

    [SyncVar] 
    [SerializeField]    // TODO: Remove later
    private Constants.Team team = Constants.Team.Missing;

    [SyncVar] 
    [SerializeField]    // TODO: Remove later
    private Constants.Element element = Constants.Element.Missing;

    public static event Action<PlayerCharacter> ClientOnMyPlayerCharacterSpawned;
    public static event Action<PlayerCharacter> ClientOnMyPlayerCharacterDespawned;

    public static Action<int> ServerOnPlayerCharacterDie;
    public static event Action<PlayerCharacter> ServerOnPlayerCharacterSpawned;
    public static event Action<PlayerCharacter> ServerOnPlayerCharacterDespawned;

    private void OnEnable()
    {
        PauseMenu.ClientStartPause += ClientHandleStartPause;
        PauseMenu.ClientEndPause += ClientHandleEndPause;
        SelectElementUI.ClientCloseSelectElementUI += ClientHandleEndPause;
        GameManager.ClientOnRoundWin += ClientHandleRoundWin;
        GameManager.ClientOnGameStart += ClientHandleGameStart;
    }
    private void OnDisable()
    {
        PauseMenu.ClientStartPause -= ClientHandleStartPause;
        PauseMenu.ClientEndPause -= ClientHandleEndPause;
        SelectElementUI.ClientCloseSelectElementUI -= ClientHandleEndPause;
        GameManager.ClientOnRoundWin -= ClientHandleRoundWin;
        GameManager.ClientOnGameStart -= ClientHandleGameStart;
    }

    private void Start()
    {
        if (!hasAuthority) { return; }


        ClientOnMyPlayerCharacterSpawned?.Invoke(this);

        ResetCharacterRotation();

        if (!PauseMenu.IsInPauseMenu)
        {
            ClientHandleEndPause();
            return; 
        }

        ClientHandleStartPause();
    }

    
    //public override void OnStopAuthority()
    //{
    //    ClientOnMyPlayerCharacterDespawned?.Invoke(this);
    //}

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }

        ClientOnMyPlayerCharacterDespawned?.Invoke(this);

        //Debug.Log(hasAuthority);
    }

    #region Getters/Setters
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
    #endregion

#region Server

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
        ServerOnPlayerCharacterDie?.Invoke(connectionToClient.connectionId);    // Destroy all player owned objects with health

        // What do we actually want to do when we die
        FPSPlayer fpsPlayer = connectionToClient.identity.GetComponent<FPSPlayer>();
        fpsPlayer.SetActivePlayerCharacter(null);   // Helps for logic when respawning (null = no active playerCharacter)

        // We're in midgame

        // So that players are removed from clients
        // Not sure why this fixes it - maybe because players have authority over themselves
        netIdentity.serverOnly = true;

        // Destroying player character
        NetworkServer.Destroy(gameObject);

        if (GameManager.singleton.IsGameInProgress()) { return; }

        // We're in pregame

        // If not becoming spectator, then respawn player
        //Debug.Log(fpsPlayer.HasActiveSpecCamera());

        if (fpsPlayer.HasActiveSpecCamera()) { return; }

        // Respawning player character 
        fpsPlayer.RespawnPlayer();  // If game is not in progress, it's pregame and respawn character

    }

    #endregion

    #region Client


    [Client]
    private void ClientHandleStartPause()
    {
        ChangeActiveState(false);
    }

    [Client]
    private void ClientHandleEndPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Debug.Log("Ending pause!");
        ChangeActiveState(true);
    }

    [Client]
    private void ClientHandleGameStart()
    {
        ResetCharacterRotation();
    }

    [Client]
    private void ClientHandleRoundWin()
    {
        ResetCharacterRotation();
    }

    [Client]
    private void ResetCharacterRotation()
    {
        if (!hasAuthority) { return; }

        var gm = GameManager.singleton;

        switch (team)
        {
            case Constants.Team.Red:
                GetComponentInChildren<PlayerAiming>().SetCurrentRotation(gm.GetRedSpawn().rotation.eulerAngles);
                break;
            case Constants.Team.Blue:
                GetComponentInChildren<PlayerAiming>().SetCurrentRotation(gm.GetBlueSpawn().rotation.eulerAngles);
                break;
            case Constants.Team.Spectator:
                Debug.Log("Player character cannot be spectator!. Error has occurred.");
                break;
            case Constants.Team.Missing:
                Debug.Log("Invalid Team. Error has occurred.");
                break;
        }
    }

    private void ChangeActiveState(bool newState)
    {
        if (!hasAuthority) { return; }  // Only disable for local player

        // Movement disabled in SurfCharacter script due to limitations

        PlayerAiming playerAiming = GetComponentInChildren<PlayerAiming>();
        ArmSway itemSway = GetComponentInChildren<ArmSway>();
        ItemBob itemBob = GetComponentInChildren<ItemBob>();


        // Check because player might be dead

        if (playerAiming != null) { playerAiming.enabled = newState; }
        if (itemSway != null) { itemSway.enabled = newState; }
        if (itemBob != null) { itemBob.enabled = newState; }


        var animators = GetComponentsInChildren<Animator>();

        foreach (Animator animator in animators)
        {
            animator.enabled = newState;
        }

        // TODO: Add ability component

    }
    #endregion
}
