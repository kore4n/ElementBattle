using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnTriggerable : NetworkBehaviour
{
    [HideInInspector] public GameObject blockAreaPrefab;
    public Transform blockAreaSpawn;

    public void Spawn()
    {
        GameObject shieldInstance = Instantiate(blockAreaPrefab, blockAreaSpawn);

        NetworkServer.Spawn(shieldInstance, connectionToClient);
    }
}
