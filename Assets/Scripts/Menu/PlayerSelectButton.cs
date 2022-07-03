using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectButton : MonoBehaviour
{
    private FPSPlayer player;
    [SerializeField] private GameObject elementSelectionParent;

    void Start()
    {
        FPSPlayer.OnPlayerSpawn += HandlePlayerSpawn;
    }

    private void OnDestroy()
    {
        FPSPlayer.OnPlayerSpawn -= HandlePlayerSpawn;
    }

    // Shouldn't have to do this but I do.
    // Placing line in Start makes it run too early
    private void HandlePlayerSpawn()
    {
        player = NetworkClient.connection.identity.GetComponent<FPSPlayer>();
    }

    private void MakePlayer(Constants.Element elementType)
    {
        PlayerInfo playerInfo = new PlayerInfo()
        {
            element = elementType,
        };

        player.CmdCreatePlayerCharacter(playerInfo);

        elementSelectionParent.SetActive(false);

        Camera.main.gameObject.SetActive(false);
    }

    public void MakePlayerWater()
    {
        MakePlayer(Constants.Element.Water);
    }
    public void MakePlayerEarth()
    {
        MakePlayer(Constants.Element.Earth);
    }
    public void MakePlayerFire()
    {
        MakePlayer(Constants.Element.Fire);
    }
    public void MakePlayerAir()
    {
        MakePlayer(Constants.Element.Air);
    }
}

public struct PlayerInfo : NetworkMessage
{
    public Constants.Element element;
    public string playerName;
}