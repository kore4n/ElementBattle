using Mirror;
using System.Collections;
using UnityEngine;

namespace Game.Abilities
{
    public class StructureSpawnTriggerable : NetworkBehaviour
    {
        [HideInInspector] public GameObject structurePrefab;

        [SerializeField] Transform spawnPos;
        [SerializeField] LayerMask groundLayerMask;

        [Server]
        public void Spawn()
        {
            if (!FindSpawnableSurface(spawnPos.position, out Vector3 surfaceHitPoint)) return;

            GameObject structureInstance = Instantiate(structurePrefab, surfaceHitPoint, transform.rotation);

            NetworkServer.Spawn(structureInstance, connectionToClient);
        }

        private bool FindSpawnableSurface(Vector3 point, out Vector3 surfaceHitPoint)
        {
            surfaceHitPoint = Vector3.zero;

            if (!Physics.Raycast(new Ray(point, Vector3.down), out RaycastHit hit, Mathf.Infinity, groundLayerMask)) return false;

            surfaceHitPoint = hit.point;
            return true;
        }
    }
}