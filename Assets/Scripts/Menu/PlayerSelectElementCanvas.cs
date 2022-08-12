using Mirror;
using TMPro;
using UnityEngine;

public class PlayerSelectElementCanvas : MonoBehaviour
{
    private FPSPlayer localPlayer;
    [SerializeField] private GameObject elementSelectionParent;

    [SerializeField] private TextMeshProUGUI[] availableElementText = new TextMeshProUGUI[Constants.numberOfElements];

    private void OnEnable()
    {
        FPSPlayer.OnPlayerSpawn += HandlePlayerSpawn;
        FPSPlayer.ClientOnMeChooseElement += ClientHandleMeChooseElement;
        FPSPlayer.ClientOnAnyoneChooseElement += UpdateAnyoneChooseElement;
        SelectTeamUI.MeSelectedTeam += UpdateAnyoneChooseElement;
        //FPSPlayer.ClientOnMeChooseTeam += UpdateAnyoneChooseElement;
        FPSPlayer.ClientOnMeChooseTeam += ClientHandleMeChooseTeam;
    }

    private void OnDisable()
    {
        FPSPlayer.OnPlayerSpawn -= HandlePlayerSpawn;
        FPSPlayer.ClientOnMeChooseElement -= ClientHandleMeChooseElement;
        FPSPlayer.ClientOnAnyoneChooseElement -= UpdateAnyoneChooseElement;
        SelectTeamUI.MeSelectedTeam -= UpdateAnyoneChooseElement;
        //FPSPlayer.ClientOnMeChooseTeam -= UpdateAnyoneChooseElement;
    }

    // Shouldn't have to do this but I do.
    // Placing line in Start makes it run too early
    private void HandlePlayerSpawn()
    {
        localPlayer = NetworkClient.connection.identity.GetComponent<FPSPlayer>();
    }

    private void SetPlayerElement(Constants.Element elementType)
    {
        PlayerInfo playerInfo = new PlayerInfo()
        {
            element = elementType,
        };

        localPlayer.CmdChooseElement(playerInfo);

    }

    [Client]
    private void ClientHandleMeChooseElement()
    {
        elementSelectionParent.SetActive(false);

        Camera.main.gameObject.SetActive(false);
    }

    [Client]
    private void UpdateAnyoneChooseElement()
    {
        for (int i = 0; i < Constants.numberOfElements; i++)
        {
            availableElementText[i].text = "0/1 (Empty)";
        }

        foreach (FPSPlayer curPlayer in ((FPSNetworkManager)NetworkManager.singleton).players)
        {
            var curPlayerTeam = curPlayer.GetTeam();

            if (curPlayer.GetElement() == Constants.Element.Missing) { continue; }
            if (curPlayerTeam != localPlayer.GetTeam()) { continue; }

            availableElementText[(int)curPlayer.GetElement()].text = "1/1 (Full)";
        }
    }

    [Client]
    private void ClientHandleMeChooseTeam(Constants.Team team)
    {
        for (int i = 0; i < Constants.numberOfElements; i++)
        {
            availableElementText[i].text = "0/1 (Empty)";
        }

        foreach (FPSPlayer curPlayer in ((FPSNetworkManager)NetworkManager.singleton).players)
        {
            var curPlayerTeam = curPlayer.GetTeam();

            if (curPlayer.GetElement() == Constants.Element.Missing) { continue; }
            if (curPlayerTeam != team) { continue; }

            availableElementText[(int)curPlayer.GetElement()].text = "1/1 (Full)";
        }
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