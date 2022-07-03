using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSSteamManager : MonoBehaviour
{
    private void Awake()
    {
        try
        {
            SteamClient.Init(480);
        }
        catch (Exception e)
        {
            Debug.Log($"Couldn't initialize steam client. Reason: {e}");
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        SteamClient.Shutdown();
    }

    void Update()
    {
        SteamClient.RunCallbacks();
    }
}
