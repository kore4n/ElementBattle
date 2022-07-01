using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

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
        NetworkManager.singleton.StartClient();
    }
}
