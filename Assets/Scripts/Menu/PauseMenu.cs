using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private PlayerCharacter localPlayerCharacter;

    [SerializeField] private GameObject canvas;

    private bool IsInPauseMenu = false;

    private void Start()
    {
        // In case forgot to set to active in editor/for testing
        canvas.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            localPlayerCharacter = NetworkClient.connection.identity.GetComponent<FPSPlayer>().GetActivePlayerCharacter();

            //PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();
            //foreach (PlayerCharacter playerCharacter in playerCharacters)
            //{
            //    if (!playerCharacter.hasAuthority) { continue; }

            //    localPlayerCharacter = playerCharacter;
            //}

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
        ToggleIt(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ClosePauseMenu()
    {
        ToggleIt(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ToggleIt(bool newState)
    {
        IsInPauseMenu = !newState;
        canvas.SetActive(!newState);

        // Check because player might be dead
        if (localPlayerCharacter.TryGetComponent(out PlayerAiming playerAiming))
        {
            playerAiming.enabled = newState;
        }
        if (localPlayerCharacter.TryGetComponent(out ArmSway itemSway))
        {
            itemSway.enabled = newState;
        }
        if (localPlayerCharacter.TryGetComponent(out ItemBob itemBob))
        {
            itemBob.enabled = newState;
        }

        //localPlayerCharacter.GetComponentInChildren<PlayerAiming>().enabled = newState;
        //localPlayerCharacter.GetComponentInChildren<ArmSway>().enabled = newState;
        //localPlayerCharacter.GetComponentInChildren<ItemBob>().enabled = newState;
        // TODO: Just disable movement, line below freezes player midair
        //localPlayerCharacter.GetComponentInChildren<MyCharacterMovement>().enabled = newState;
        var animators = localPlayerCharacter.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = newState;
        }

        // TODO: Add ability component
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
}
