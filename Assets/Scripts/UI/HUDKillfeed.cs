using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDKillfeed : MonoBehaviour
{
    [SerializeField] private GameObject canvas = null;
    [SerializeField] private GameObject HUDKillfeedSlotPrefab = null;

    void Start()
    {
        PlayerCharacter.ServerOnPlayerCharacterDespawned += ClientHandlePlayerDeath;
    }

    private void OnDestroy()
    {
        PlayerCharacter.ServerOnPlayerCharacterDespawned -= ClientHandlePlayerDeath;
    }

    public void ClientHandlePlayerDeath(PlayerCharacter playerCharacter)
    {
        HUDKillfeedSlot hUDKillfeedSlot = Instantiate(HUDKillfeedSlotPrefab).GetComponent<HUDKillfeedSlot>();

        //if (NetworkClient.connection.identity.GetComponent<PlayerCharacter>().playerCharacterName == playerCharacter.playerCharacterName)
        //{ hUDKillfeedSlot.SetBackgroundImageColor(Color.white); }

        hUDKillfeedSlot.SetDeathNameColor(playerCharacter.GetTeam());
        hUDKillfeedSlot.SetDeathNameText(playerCharacter.playerCharacterName);
        hUDKillfeedSlot.transform.SetParent(canvas.transform);
    }
}
