using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterAction : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileSpawnPoint;

    //[ClientCallback]
    //void Update()
    //{
    //    if (Mouse.current.leftButton.isPressed)
    //    {
    //        SpawnProjectile();
    //    }
    //}

    //[Command]
    //private void SpawnProjectile()
    //{

    //    GameObject projectileInstance = Instantiate(
    //        projectilePrefab, projectileSpawnPoint.position, projectileRotation);

    //    NetworkServer.Spawn(projectileInstance, connectionToClient);
    //}
}
