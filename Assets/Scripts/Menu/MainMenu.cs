using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField addressInput = null;

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
}
