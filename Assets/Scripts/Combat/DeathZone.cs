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
            //rotation.eulerAngles = new Vector3(playerCharacterObject.transform.eulerAngles.x, playerCamera.transform.localEulerAngles.y, 0f);
            rotation.eulerAngles = new Vector3(playerCharacterObject.transform.eulerAngles.y, playerCamera.transform.localEulerAngles.x, 0f);
            //Debug.Log(playerCharacterObject.transform.eulerAngles.y);
            //Debug.Log(playerCamera.transform.localEulerAngles.x);

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

            playerHealth.DealDamage(killBoxDamage);

        }
    }
}
