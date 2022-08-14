using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private PlayerCharacter localPlayerCharacter;

    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject teamSelectCanvas;
    [SerializeField] private GameObject elementSelectCanvas;

    public static bool IsInPauseMenu = false;

    public static Action ClientStartPause;
    public static Action ClientEndPause;

    private void Start()
    {
        // In case forgot to set to active in editor/for testing
        pauseCanvas.SetActive(false);
        IsInPauseMenu = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //localPlayerCharacter = NetworkClient.connection.identity.GetComponent<FPSPlayer>().GetActivePlayerCharacter();

            PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();
            foreach (PlayerCharacter playerCharacter in playerCharacters)
            {
                //Debug.Log(playerCharacter);
                if (!playerCharacter.hasAuthority) { continue; }

                localPlayerCharacter = playerCharacter;
                //Debug.Log($"Setting local player to {playerCharacter}! Is actually {localPlayerCharacter}...");
            }

            if (!IsInPauseMenu)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        ClientStartPause?.Invoke();
        ToggleIt(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ClosePauseMenu()
    {
        ClientEndPause?.Invoke();
        ToggleIt(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ToggleIt(bool newState)
    {
        IsInPauseMenu = newState;
        pauseCanvas.SetActive(newState);
    }

    public void DisconnectFromServer()
    {

        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    public void OpenSelectTeamUI()
    {
        teamSelectCanvas.SetActive(true);
        pauseCanvas.SetActive(false);

    }

    public void CloseSelectTeamUI()
    {
        teamSelectCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
    }

    public void OpenSelectElementUI()
    {
        // Don't allow to open up element select if not red or blue
        // There is a failsafe in "CmdChooseElement" in FPSplayer but do this in case
        FPSPlayer player = NetworkClient.connection.identity.GetComponent<FPSPlayer>(); // Don't have to check because this has to exist
        if (player.GetTeam() == Constants.Team.Spectator || player.GetTeam() == Constants.Team.Missing) { return; }

        elementSelectCanvas.SetActive(true);
        pauseCanvas.SetActive(false);

    }

    public void CloseSelectElementUI()
    {
        elementSelectCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
    }
}
