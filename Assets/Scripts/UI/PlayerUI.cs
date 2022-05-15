using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public GameObject menuCamera;
        public GameObject buttonsParent;

        public Constants.Element element;
        public GameObject playerHUD;
        public Image playerElement;

        // Ensure order: Water, Earth, Fire, Air; same as in Constants class
        public Sprite[] elementSprites;
        public Button[] buttons;

        /// <summary>
        /// Occurs upon button click. Set up player info for UI.
        /// </summary>
        /// <param name="info"></param>
        public void CreatePlayer(CreatePlayerMessage info)
        {
            // Disable menu
            if (menuCamera != null)
            {
                menuCamera.SetActive(false);
            }
            if (buttonsParent != null)
            {
                buttonsParent.SetActive(false);
            }

            // TODO: Set more player info
            Constants.Element element = info.element;

            // Set player sprite in UI
            playerElement.sprite = elementSprites[(int)element];
        }

        public void Awake()
        {
            DontDestroyOnLoad(this);

            SubscribeElementButtons();
        }

        // Subscribe to all element-select buttons
        private void SubscribeElementButtons()
        {
            // Ensure order: Water, Earth, Fire, Air
            SubscribeToButton(buttons[0], "fred", Constants.Element.water);
            SubscribeToButton(buttons[1], "fred", Constants.Element.earth);
            SubscribeToButton(buttons[2], "fred", Constants.Element.fire);
            SubscribeToButton(buttons[3], "fred", Constants.Element.air);
        }

        /// <summary>
        /// Subscribe to button press for character element select.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="name"></param>
        /// <param name="element"></param>
        public void SubscribeToButton(Button button, string name, Constants.Element element)
        {
            CreatePlayerMessage playerInfo = new CreatePlayerMessage();
            playerInfo.name = name;
            playerInfo.element = element;

            button.onClick.AddListener(delegate { CreatePlayer(playerInfo); });
        }

        public void ShowPlayerHud()
        {
            playerHUD.SetActive(true);
        }
    }
}
