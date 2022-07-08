using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;
    [SerializeField] private TMP_Text subText = null;

    private void Start()
    {
        GameManager.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameManager.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(Constants.Team winner)
    {
        if (winner == NetworkClient.connection.identity.GetComponent<FPSPlayer>().GetTeam())
        {
            winnerNameText.text = $"You won!";
            subText.text = "(you suck less)";
        }
        else
        {
            winnerNameText.text = $"{winner} team has won!";
            subText.text = "(you suck)";
        }

        gameOverDisplayParent.SetActive(true);
    }
}