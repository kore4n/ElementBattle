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


        public void ShowPlayerHud()
        {
            playerHUD.SetActive(true);
        }
    }
}
