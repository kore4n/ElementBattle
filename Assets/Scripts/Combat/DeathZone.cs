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

            FPSNetworkManager networkManager = (FPSNetworkManager)NetworkManager.singleton;

            networkManager.SpawnSpectatorCamera(playerCamera.transform.position, rotation, networkConnectionToClient);

            Debug.Log("Killing player!");
            playerHealth.DealDamage(killBoxDamage);

        }
    }

    [Server]
    private void DestroyObject(GameObject gameObject)
    {
        NetworkServer.Destroy(gameObject);
    }
}
