using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<PlayerCharacter>(out PlayerCharacter playerCharacter))
        {
            DestroyObject(other.transform.parent.gameObject);

            GameObject spectatorCamera = Instantiate(((FPSNetworkManager)NetworkManager.singleton).GetSpectatorCamera());
            NetworkServer.Spawn(spectatorCamera, playerCharacter.connectionToClient);
        }
    }

    [Server]
    private void DestroyObject(GameObject gameObject)
    {
        NetworkServer.Destroy(gameObject);
    }
}
