using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTeamUI : MonoBehaviour
{
    FPSPlayer player = null;
    [SerializeField] private GameObject teamSelectionUI = null;
    [SerializeField] private GameObject elementSelectionUI = null;

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

    public void MakePlayerRed()
    {
        MakePlayerTeam(Constants.Team.Red);
    }
    public void MakePlayerBlue()
    {
        MakePlayerTeam(Constants.Team.Blue);
    }
    public void MakePlayerSpectator()
    {
        MakePlayerTeam(Constants.Team.Spectator);
    }

    private void MakePlayerTeam(Constants.Team team)
    {
        elementSelectionUI.SetActive(true);
        teamSelectionUI.SetActive(false);

        player.CmdSetTeam(team);
    }
}
