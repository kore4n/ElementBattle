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

    private void HandlePlayerSpawn()
    {
        player = NetworkClient.connection.identity.GetComponent<FPSPlayer>();
    }

    private void MakePlayer(Constants.Element elementType)
    {
        PlayerInfo playerInfo = new PlayerInfo()
        {
            playerName = "Test Name",
            element = elementType,
        };

        player.CmdCreatePlayerCharacter(playerInfo);

        elementSelectionParent.SetActive(false);

        Camera.main.gameObject.SetActive(false);
    }

    public void MakePlayerWater()
    {
        MakePlayer(Constants.Element.water);
    }
    public void MakePlayerEarth()
    {
        MakePlayer(Constants.Element.earth);
    }
    public void MakePlayerFire()
    {
        MakePlayer(Constants.Element.fire);
    }
    public void MakePlayerAir()
    {
        MakePlayer(Constants.Element.air);
    }
}

public struct PlayerInfo : NetworkMessage
{
    public Constants.Element element;
    public string playerName;
}