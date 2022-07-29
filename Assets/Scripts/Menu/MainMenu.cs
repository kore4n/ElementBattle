using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField addressInput = null;

    [SerializeField] private GameObject startPanel = null;
    [SerializeField] private GameObject multiplayerPanel = null;
    [SerializeField] private GameObject connectPanel = null;

    public void ButtonHost()
    {
        NetworkManager.singleton.StartHost();
    }

    public void ButtonServer()
    {
        NetworkManager.singleton.StartServer();
    }

    public void ButtonClient()
    {
        string finalAddress = "";

        // If address field not empty then use custom address
        if (addressInput.text.Trim() != "") { finalAddress = addressInput.text; }

        NetworkManager.singleton.networkAddress = finalAddress;
        NetworkManager.singleton.StartClient();
    }

    #region OpenClose
    public void OpenMultiplayerPanel()
    {
        multiplayerPanel.SetActive(true);
        startPanel.SetActive(false);
    }
    public void CloseMultiplayerPanel()
    {
        multiplayerPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void OpenConnectPanel()
    {
        connectPanel.SetActive(true);
        multiplayerPanel.SetActive(false);
    }

    public void CloseConnectPanel()
    {
        connectPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
    }
    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
}
