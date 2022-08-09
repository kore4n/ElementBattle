using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersConnectedHUD : MonoBehaviour
{
    private List<GameObject> playersConnectedSlotHUD = new List<GameObject>();

    [SerializeField] private GameObject canvas = null;
    [SerializeField] private GameObject playerSlotHUD = null;

    private void OnEnable()
    {
        FPSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        GameManager.ClientOnGameStart += ClientHandleGameStart;
        GameManager.ClientOnGameOver += ClientHandleGameOver;
        GameManager.OnGameManagerSpawn += ClientHandleGameManagerSpawn;
    }

    private void OnDisable()
    {
        FPSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        GameManager.ClientOnGameStart -= ClientHandleGameStart;
        GameManager.ClientOnGameOver -= ClientHandleGameOver;
        GameManager.OnGameManagerSpawn -= ClientHandleGameManagerSpawn;
    }

    private void Start()
    {
        
    }

    private void ClientHandleGameManagerSpawn()
    {
        if (!GameManager.singleton.IsGameInProgress()) { return; }

        // Hide HUD if game in progress
        ClientHandleGameStart();
    }

    private void ClientHandleInfoUpdated()
    {
        ClearConnectedPlayersHUD();

        List<FPSPlayer> players = ((FPSNetworkManager)NetworkManager.singleton).players;

        foreach (FPSPlayer p in players)
        {
            if (p.GetTeam() == Constants.Team.Spectator) { continue; }  // Don't make a slot for spectators

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

    private void ClientHandleGameOver(Constants.Team winner)
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
