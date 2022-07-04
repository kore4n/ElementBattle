using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText = null;
    [SerializeField] private Image imageSprite = null;
    [SerializeField] private GameObject readyCheckmark = null;
    [SerializeField] private Image teamBackgroundImage = null;

    public void SetName(string newName)
    {
        nameText.text = newName;
    }

    public void SetSprite(Sprite newSprite)
    {
        imageSprite.sprite = newSprite;
    }

    public void SetReady(bool readyState)
    {
        readyCheckmark.SetActive(readyState);
    }

    public void SetTeam(Constants.Team newTeam)
    {
        switch (newTeam)
        {
            case Constants.Team.Red:
                teamBackgroundImage.gameObject.SetActive(true);
                teamBackgroundImage.color = Color.red;
                break;
            case Constants.Team.Blue:
                teamBackgroundImage.gameObject.SetActive(true);
                teamBackgroundImage.color = Color.blue;
                break;
            case Constants.Team.Spectator:
                teamBackgroundImage.gameObject.SetActive(false);
                break;
        }
    }
}
