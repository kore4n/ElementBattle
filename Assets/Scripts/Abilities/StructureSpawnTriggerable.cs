using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class StructureSpawnTriggerable : NetworkBehaviour
    {
        [HideInInspector] public GameObject structurePrefab;
        [HideInInspector] public int maxStructureInstances;

        [SerializeField] Transform spawnPos;
        [SerializeField] LayerMask groundLayerMask;
        [SerializeField] LayerMask structureLayerMask;
        [SerializeField] float maxDistanceFromSurface = Mathf.Infinity;

        readonly List<Structure> structureInstances = new();

        Bounds structurePrefabBounds;

        [Server]
        public void Spawn()
        {
            // This is needed to remove any destroyed structures that may still be in the list
            structureInstances.RemoveAll(item => item == null);

            if (structureInstances.Count == maxStructureInstances)
            {
                Structure firstStructure = structureInstances[0];
                structureInstances.RemoveAt(0);
                firstStructure.DestroySelf();
                return;
            }

            if (!FindSpawnableSurface(spawnPos.position, out Vector3 surfaceHitPoint)) return;

            GameObject structureInstance = Instantiate(structurePrefab, surfaceHitPoint, transform.rotation);
            structureInstances.Add(structureInstance.GetComponent<Structure>());

            NetworkServer.Spawn(structureInstance, connectionToClient);
        }

        /// <summary>
        /// Finds a suitable surface point below the given point to spawn the structurePrefab if possible
        /// </summary>
        /// <param name="point">Point in world space to spawn</param>
        /// <param name="surfaceHitPoint">Point on a valid surface to spawn the structurePrefab</param>
        /// <returns>Whether or not the given point yields a valid surfaceHitPoint</returns>
        private bool FindSpawnableSurface(Vector3 point, out Vector3 surfaceHitPoint)
        {
            if (structurePrefabBounds == null) LoadStructurePrefabBounds();

            surfaceHitPoint = Vector3.zero;

            if (!Physics.Raycast(new Ray(point, Vector3.down), out RaycastHit hit, maxDistanceFromSurface, groundLayerMask)) return false;

            surfaceHitPoint = hit.point;

            if (Physics.CheckBox(surfaceHitPoint + structurePrefabBounds.center, structurePrefabBounds.extents, transform.rotation, structureLayerMask)) return false;

            return true;
        }

        private void LoadStructurePrefabBounds()
        {
            // Wack thing to get the bounds because you need to instantiate a prefab to get it's bounds
            GameObject s = Instantiate(structurePrefab);
            structurePrefabBounds = s.GetComponent<Collider>().bounds;
            DestroyImmediate(s);
        }
    }
}