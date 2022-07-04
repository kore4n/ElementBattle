using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersConnectedHUD : MonoBehaviour
{
    private List<GameObject> playersConnectedSlotHUD = new List<GameObject>();

    [SerializeField] private GameObject canvas = null;
    [SerializeField] private GameObject playerSlotHUD = null; 

    private void Start()
    {
        FPSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        GameOverHandler.ServerOnGameStart += ClientHandleGameStart;
        GameOverHandler.ServerOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        FPSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        GameOverHandler.ServerOnGameStart -= ClientHandleGameStart;
        GameOverHandler.ServerOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleInfoUpdated()
    {
        ClearConnectedPlayersHUD();

        List<FPSPlayer> players = ((FPSNetworkManager)NetworkManager.singleton).players;

        foreach (FPSPlayer p in players)
        {
            GameObject newSlotHUD = Instantiate(playerSlotHUD);
            PlayerSlotHUD newPlayerSlotHUD = newSlotHUD.GetComponent<PlayerSlotHUD>();
            newPlayerSlotHUD.SetName(p.GetDisplayName());
            newPlayerSlotHUD.SetSprite(p.GetElementSprite());
            newPlayerSlotHUD.SetReady(p.GetReadiedUp());
            newPlayerSlotHUD.SetTeam(p.GetTeam());

            newSlotHUD.transform.SetParent(canvas.transform);

            playersConnectedSlotHUD.Add(newSlotHUD);
        }
    }

    private void ClientHandleGameStart()
    {
        canvas.SetActive(false);
    }

    private void ClientHandleGameOver()
    {
        canvas.SetActive(true);
    }
    private void ClearConnectedPlayersHUD()
    {
        foreach (GameObject playerSlotHUD in playersConnectedSlotHUD)
        {
            Destroy(playerSlotHUD);
        }
        playersConnectedSlotHUD.Clear();
    }
}
