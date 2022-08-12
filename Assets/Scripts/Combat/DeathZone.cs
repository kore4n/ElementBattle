using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private int killBoxDamage = 9999;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<PlayerCharacter>(out PlayerCharacter playerCharacter))
        {
            GameObject playerCharacterObject = playerCharacter.gameObject;
            GameObject playerCamera = playerCharacter.GetComponent<MyCharacterController>().GetCameraHolder();

            var rotation = new Quaternion();    // TODO: Rotation Not matching
            rotation.eulerAngles = new Vector3(playerCharacterObject.transform.eulerAngles.x, playerCamera.transform.localEulerAngles.y, 0f);

            Health playerHealth = playerCharacter.GetComponent<Health>();

            NetworkConnectionToClient networkConnectionToClient = playerCharacter.connectionToClient;


            // Respawn in pregame
            if (!GameManager.singleton.IsGameInProgress())
            {
                playerHealth.DealDamage(killBoxDamage);
                return;
            }

            // Spawn spectator camera then kill 
            // Don't change order! Or else spectator camera not added to list when player dies
            // and round may be restarted
            FPSNetworkManager networkManager = (FPSNetworkManager)NetworkManager.singleton;

            GameObject spectatorCamera = Instantiate(
                networkManager.GetSpectatorCamera(),
                playerCamera.transform.position,
                rotation);
            NetworkServer.Spawn(spectatorCamera, networkConnectionToClient);

            networkManager.spectatorCameras.Add(spectatorCamera.GetComponent<SpectatorCameraController>());
            playerHealth.DealDamage(killBoxDamage);

        }
    }

    [Server]
    private void DestroyObject(GameObject gameObject)
    {
        NetworkServer.Destroy(gameObject);
    }
}
