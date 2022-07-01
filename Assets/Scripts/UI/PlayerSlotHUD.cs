using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image imageSprite;

    public void SetName(string newName)
    {
        nameText.text = newName;
    }

    public void SetSprite(Sprite newSprite)
    {
        imageSprite.sprite = newSprite;
    }
}
