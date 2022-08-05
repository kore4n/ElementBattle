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

            // Respawn in pregame
            if (!GameManager.singleton.IsGameInProgress())
            {
                playerHealth.DealDamage(killBoxDamage);
                return;
            }

            GameObject spectatorCamera = Instantiate(
                ((FPSNetworkManager)NetworkManager.singleton).GetSpectatorCamera(),
                playerCamera.transform.position,
                rotation);
            NetworkServer.Spawn(spectatorCamera, playerCharacter.connectionToClient);

            playerHealth.DealDamage(killBoxDamage);
        }
    }

    [Server]
    private void DestroyObject(GameObject gameObject)
    {
        NetworkServer.Destroy(gameObject);
    }
}
