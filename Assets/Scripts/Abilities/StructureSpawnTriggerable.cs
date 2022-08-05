using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class StructureSpawnTriggerable : NetworkBehaviour
    {
        [SerializeField] Transform spawnPos;
        [SerializeField] LayerMask groundLayerMask;
        [SerializeField] LayerMask structureLayerMask;
        [SerializeField] float maxDistanceFromSurface = Mathf.Infinity;

        readonly Dictionary<int, Bounds> structurePrefabBounds = new();

        [Server]
        public Structure Spawn(GameObject structurePrefab)
        {
            if (!FindSpawnableSurface(structurePrefab, spawnPos.position, out Vector3 surfaceHitPoint)) return null;

            GameObject structureInstance = Instantiate(structurePrefab, surfaceHitPoint, transform.rotation);

            NetworkServer.Spawn(structureInstance, connectionToClient);

            return structureInstance.GetComponent<Structure>();
        }

        /// <summary>
        /// Finds a suitable surface point below the given point to spawn the structurePrefab if possible
        /// </summary>
        /// <param name="point">Point in world space to spawn</param>
        /// <param name="surfaceHitPoint">Point on a valid surface to spawn the structurePrefab</param>
        /// <returns>Whether or not the given point yields a valid surfaceHitPoint</returns>
        private bool FindSpawnableSurface(GameObject structurePrefab, Vector3 point, out Vector3 surfaceHitPoint)
        {
            if (!structurePrefabBounds.ContainsKey(structurePrefab.GetInstanceID()))
            {
                LoadStructurePrefabBounds(structurePrefab);
            }

            Bounds structureBound = structurePrefabBounds[structurePrefab.GetInstanceID()];

            surfaceHitPoint = Vector3.zero;

            if (!Physics.Raycast(new Ray(point, Vector3.down), out RaycastHit hit, maxDistanceFromSurface, groundLayerMask)) return false;

            surfaceHitPoint = hit.point;

            if (Physics.CheckBox(surfaceHitPoint + structureBound.center, structureBound.extents, transform.rotation, structureLayerMask)) return false;

            return true;
        }

        private void LoadStructurePrefabBounds(GameObject structurePrefab)
        {
            // Wack thing to get the bounds because you need to instantiate a prefab to get it's bounds
            GameObject s = Instantiate(structurePrefab);
            structurePrefabBounds[structurePrefab.GetInstanceID()] = s.GetComponent<Collider>().bounds;
            DestroyImmediate(s);
        }
    }
}