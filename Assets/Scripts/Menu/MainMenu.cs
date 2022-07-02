using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private TMP_InputField nameInput = null;

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
        NetworkManager.singleton.networkAddress = addressInput.text;
        NetworkManager.singleton.StartClient();
    }
}
