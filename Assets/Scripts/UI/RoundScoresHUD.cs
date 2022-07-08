using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundScoresHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text redRoundsText = null;
    [SerializeField] private TMP_Text blueRoundsText = null;

    void Start()
    {
        GameManager.ClientOnRoundWin += ClientHandleRoundWin;
    }

    private void OnDestroy()
    {
        GameManager.ClientOnRoundWin -= ClientHandleRoundWin;
    }
    
    private void ClientHandleRoundWin()
    {
        redRoundsText.text = GameManager.singleton.redRounds.ToString();
        blueRoundsText.text = GameManager.singleton.blueRounds.ToString();
    }
}
