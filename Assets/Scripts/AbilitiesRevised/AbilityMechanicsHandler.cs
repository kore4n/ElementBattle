using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles all specific mechanics with every ability
/// </summary>
public class AbilityMechanicsHandler : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask groundLayerMask = new();
    [SerializeField] Transform cameraHolder;

    [Header("Projectile References")]
    [SerializeField] Transform projectileSpawnPoint;

    [Header("Structure References")]
    [SerializeField] Transform structureSpawnPoint;
    [SerializeField] LayerMask structureLayerMask = new();
    private Dictionary<int, List<Structure>> structureInstanceMap = new();
    private Dictionary<int, Bounds> structurePrefabBounds = new();

    [Header("Blocker References")]
    [SerializeField] Transform blockSpawnParent;

    #region General

    [Server]
    public GameObject SpawnPlayerObject(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab);

        NetworkServer.Spawn(instance, connectionToClient);

        return instance;
    }

    [Server]
    public GameObject SpawnPlayerObject(GameObject prefab, Transform parent)
    {
        GameObject instance = Instantiate(prefab, parent);

        NetworkServer.Spawn(instance, connectionToClient);

        return instance;
    }

    [Server]
    public GameObject SpawnPlayerObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(prefab, position, rotation);

        NetworkServer.Spawn(instance, connectionToClient);

        return instance;
    }

    [Server]
    public GameObject SpawnPlayerObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(prefab, position, rotation, parent);

        NetworkServer.Spawn(instance, connectionToClient);

        return instance;
    }

    #endregion

    #region Projectiles

    [Server]
    public void LaunchProjectile(GameObject projectilePrefab)
    {
        SpawnPlayerObject(projectilePrefab, projectileSpawnPoint.position, cameraHolder.rotation);
    }

    #endregion

    #region Structures

    public void SpawnStructure(int structureAbilityId, int maxStructureInstances, GameObject structurePrefab, float maxDistanceFromSurface)
    {
        // Initialize instance list for first time
        if (!structureInstanceMap.ContainsKey(structureAbilityId))
        {
            structureInstanceMap[structureAbilityId] = new List<Structure>();
        }
        List<Structure> structureInstanceList = structureInstanceMap[structureAbilityId];

        // This is needed to remove any destroyed structures that may still be in the list
        structureInstanceList.RemoveAll(item => item == null);

        if (structureInstanceList.Count >= maxStructureInstances) return;

        if (!FindSpawnableSurface(structureAbilityId, structurePrefab, structureSpawnPoint.position, structureSpawnPoint.rotation, out Vector3 hitPoint, maxDistanceFromSurface)) return;
        GameObject instance = SpawnPlayerObject(structurePrefab, hitPoint, structureSpawnPoint.rotation);

        structureInstanceMap[structureAbilityId].Add(instance.GetComponent<Structure>());
    }

    /// <summary>
    /// Finds a suitable surface point below the given point to spawn the structurePrefab if possible
    /// </summary>
    /// <param name="point">Point in world space to spawn</param>
    /// <param name="surfaceHitPoint">Point on a valid surface to spawn the structurePrefab</param>
    /// <returns>Whether or not the given point yields a valid surfaceHitPoint</returns>
    private bool FindSpawnableSurface(int id, GameObject structurePrefab, Vector3 point, Quaternion structureRotation, out Vector3 surfaceHitPoint, float maxDistanceFromSurface)
    {
        if (!structurePrefabBounds.ContainsKey(id))
        {
            LoadStructurePrefabBounds(id, structurePrefab);
        }

        Bounds structureBound = structurePrefabBounds[id];

        surfaceHitPoint = Vector3.zero;

        if (!Physics.Raycast(new Ray(point, Vector3.down), out RaycastHit hit, maxDistanceFromSurface, groundLayerMask)) return false;

        surfaceHitPoint = hit.point;

        if (Physics.CheckBox(surfaceHitPoint + structureBound.center, structureBound.extents, structureRotation, structureLayerMask)) return false;

        return true;
    }

    private void LoadStructurePrefabBounds(int id, GameObject structurePrefab)
    {
        // Wack thing to get the bounds because you need to instantiate a prefab to get it's bounds
        GameObject s = Instantiate(structurePrefab);
        structurePrefabBounds[id] = s.GetComponent<Collider>().bounds;
        DestroyImmediate(s);
    }

    #endregion

    #region Blocks

    [Server]
    public void SpawnBlockObject(GameObject blockPrefab)
    {
        SpawnPlayerObject(blockPrefab, blockSpawnParent);
    }

    #endregion
}
