using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoweringArena : MonoBehaviour
{
    void Start()
    {
        FPSNetworkManager.ClientOnArenaAction += ClientHandleArenaAction;
    }

    private void OnDestroy()
    {
        FPSNetworkManager.ClientOnArenaAction -= ClientHandleArenaAction;
    }

    private void ClientHandleArenaAction(Constants.GameAction gameAction)
    {
        Debug.Log(gameAction);
    }
}
