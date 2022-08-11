using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTeamUI : MonoBehaviour
{
    private FPSPlayer player = null;
    [SerializeField] private GameObject teamSelectionUI = null;
    [SerializeField] private GameObject elementSelectionUI = null;

    public static Action MeSelectedTeam;

    private void OnEnable()
    {
        FPSPlayer.OnPlayerSpawn += HandlePlayerSpawn;
        FPSPlayer.ClientOnMeChooseTeam += ClientHandleMeChooseTeam;

    }

    private void OnDisable()
    {
        FPSPlayer.OnPlayerSpawn -= HandlePlayerSpawn;
        FPSPlayer.ClientOnMeChooseTeam -= ClientHandleMeChooseTeam;
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
        player.CmdSetTeam(team);

        

    }

    private void ClientHandleMeChooseTeam(Constants.Team team)
    {
        teamSelectionUI.SetActive(false);

        //Debug.Log("Invoking MeSelectedTeam!");
        //MeSelectedTeam?.Invoke();
        //Debug.Log($"ME TEMA IS {team}");


        //Debug.Log($"DEFINITELYME TEMA IS {player.GetTeam()}");

        // Player chose spectator
        if (team == Constants.Team.Spectator) { return; }

        elementSelectionUI.SetActive(true);
    }
}
