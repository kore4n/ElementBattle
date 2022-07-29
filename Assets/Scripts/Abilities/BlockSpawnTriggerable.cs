using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnTriggerable : NetworkBehaviour
{
    [HideInInspector] public GameObject shieldPrefab;
    public Transform shieldSpawn;

    public void Spawn()
    {
        GameObject shieldInstance = Instantiate(shieldPrefab, shieldSpawn);

        NetworkServer.Spawn(shieldInstance, connectionToClient);
    }
}
