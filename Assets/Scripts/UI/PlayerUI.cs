using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject playerHUDCanvas;
        [SerializeField] private Image playerTeamImage;
        [SerializeField] private Image playerElementImage;
        [SerializeField] private TextMeshProUGUI damageSumText;

        // Ensure order: Water, Earth, Fire, Air; same as in Constants class
        public Sprite[] elementSprites = new Sprite[4];

        private void OnEnable()
        {
            PlayerCharacter.ClientOnMyPlayerCharacterSpawned += ClientHandleMyPlayerCharacterSpawned;
            PlayerCharacter.ClientOnMyPlayerCharacterDespawned += ClientHandleMyPlayerCharacterDespawned;
        }

        private void OnDisable()
        {
            PlayerCharacter.ClientOnMyPlayerCharacterSpawned -= ClientHandleMyPlayerCharacterSpawned;
            PlayerCharacter.ClientOnMyPlayerCharacterDespawned -= ClientHandleMyPlayerCharacterDespawned;
        }

        public void ShowPlayerHUD()
        {
            playerHUDCanvas.SetActive(true);
        }

        public void HidePlayerHUD()
        {
            playerHUDCanvas.SetActive(false);
        }

        private void SetPlayerHUDSprite(Constants.Element element)
        {
            playerElementImage.sprite = elementSprites[(int)element];
        }

        private void SetPlayerHUDTeam(Constants.Team team)
        {
            switch (team)
            {
                case (Constants.Team.Red):
                    playerTeamImage.color = Constants.redTeamColor;
                    break;
                case (Constants.Team.Blue):
                    playerTeamImage.color = Constants.blueTeamColor;
                    break;
                case (Constants.Team.Spectator):
                    Debug.Log("An error has occurred.");
                    break;
                case (Constants.Team.Missing):
                    Debug.Log("An error has occurred.");
                    break;
            }
        }

        private void ClientHandleMyPlayerCharacterSpawned(PlayerCharacter playerCharacter)
        {
            SetPlayerHUDSprite(playerCharacter.GetElement());
            SetPlayerHUDTeam(playerCharacter.GetTeam());
            ShowPlayerHUD();
        }

        private void ClientHandleMyPlayerCharacterDespawned(PlayerCharacter playerCharacter)
        {
            HidePlayerHUD();
        }
    }
}
