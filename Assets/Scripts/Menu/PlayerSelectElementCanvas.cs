using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectElementCanvas : MonoBehaviour
{
    private FPSPlayer player;
    [SerializeField] private GameObject elementSelectionParent;

    void Start()
    {
        FPSPlayer.OnPlayerSpawn += HandlePlayerSpawn;
        FPSPlayer.ClientOnChooseElement += ClientHandleChooseElement;
    }

    private void OnDestroy()
    {
        FPSPlayer.OnPlayerSpawn -= HandlePlayerSpawn;
        FPSPlayer.ClientOnChooseElement -= ClientHandleChooseElement;
    }

    // Shouldn't have to do this but I do.
    // Placing line in Start makes it run too early
    private void HandlePlayerSpawn()
    {
        player = NetworkClient.connection.identity.GetComponent<FPSPlayer>();
    }

    private void SetPlayerElement(Constants.Element elementType)
    {
        PlayerInfo playerInfo = new PlayerInfo()
        {
            element = elementType,
        };

        player.CmdChooseElement(playerInfo);

    }

    [Client]
    private void ClientHandleChooseElement()
    {
        elementSelectionParent.SetActive(false);

        Camera.main.gameObject.SetActive(false);
    }

    public void MakePlayerWater()
    {
        SetPlayerElement(Constants.Element.Water);
    }
    public void MakePlayerEarth()
    {
        SetPlayerElement(Constants.Element.Earth);
    }
    public void MakePlayerFire()
    {
        SetPlayerElement(Constants.Element.Fire);
    }
    public void MakePlayerAir()
    {
        SetPlayerElement(Constants.Element.Air);
    }
}

public struct PlayerInfo : NetworkMessage
{
    public Constants.Element element;
    public string playerName;
}