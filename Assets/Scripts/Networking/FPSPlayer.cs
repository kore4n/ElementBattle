using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerCharacter;
    private GameObject activePlayerCharacter;

    public static Action OnPlayerSpawn;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    public string playerName;

    [SerializeField]     //Remove later
    private Sprite elementSprite = null;

    public static event Action ClientOnInfoUpdated;

    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite earthSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite airSprite;

    public void SetDisplayName(string name)
    {
        playerName = name;
    }

    public Sprite GetSprite()
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

        //myPlayer.GetComponent<FPSPlayer>().playerName = playerInfo.playerName;
        //myPlayer.GetComponent<FPSPlayer>().elementSprite = DetermineSprite(playerInfo.element);

        NetworkServer.Spawn(myPlayer, connectionToClient);
    }

    private Sprite DetermineSprite(Constants.Element elementType)
    {
        Sprite elementSprite = null;
        switch (elementType)
        {
            case (Constants.Element.water):
                elementSprite = waterSprite;
                break;
            case (Constants.Element.earth):
                elementSprite = earthSprite;
                break;
            case (Constants.Element.fire):
                elementSprite = fireSprite;
                break;
            case (Constants.Element.air):
                elementSprite = airSprite;
                break;
        }

        return elementSprite;
    }

    #endregion

    #region Client

    private void Start()
    {
        OnPlayerSpawn?.Invoke();
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

    #endregion
}
