using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDKillfeedSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text deathNameText = null;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private Image backgroundImage;

    private void Awake()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    public void SetDeathNameText(string playerDiedName)
    {
        deathNameText.text = $"{playerDiedName} has died!";
    }

    // TODO: Change to lighter color when you caused the death/it is you
    public void SetBackgroundImageColor(Color color)
    {

    }

    // Set to the color of the player who died
    public void SetDeathNameColor(Constants.Team playerDiedTeam)
    {
        Color newDeathNameColor = Color.yellow;
        switch (playerDiedTeam)
        {
            case Constants.Team.Red:
                newDeathNameColor = Color.red;
                break;
            case Constants.Team.Blue:
                newDeathNameColor = Color.blue;
                break;
        }

        deathNameText.color = newDeathNameColor;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
