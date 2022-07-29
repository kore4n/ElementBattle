using UnityEngine;
using Mirror;

namespace Game.Abilities
{
    public class ProjectileShootTriggerable : NetworkBehaviour
    {
        [SerializeField] GameObject cameraShoot;

        [HideInInspector] public GameObject projectilePrefab;
        public Transform projectileSpawn;

        [Server]
        public void Launch()
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
            projectileInstance.transform.forward = cameraShoot.transform.forward;

            NetworkServer.Spawn(projectileInstance, connectionToClient);
        }
    }
}