using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using Mirror;
using UnityEditor.Animations;

public class MyCharacterController : NetworkBehaviour
{
    // Water, earth, fire, air
    [SerializeField] private GameObject[] basicProjectiles = new GameObject[4];

    [SerializeField] private GameObject cameraShoot = null;

    public GameObject structurePrefab = null;
    public GameObject projectilePrefab = null;

    public GameObject GetCameraHolder()
    {
        return cameraShoot;
    }

    void Start()
    {
        if (!hasAuthority) { return; }

        Constants.Element myElement = NetworkClient.connection.identity.GetComponent<FPSPlayer>().playerElement;

        int myElementIndex = (int)myElement;
        projectilePrefab = basicProjectiles[myElementIndex];
    }

    void Update()
    {
        if (!hasAuthority) { return; }

        if (!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        // SpawnBaseProjectileCommand();
    }

    #region Server

    [Command]
    private void SpawnBaseProjectileCommand()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.transform.forward = cameraShoot.transform.forward;

        NetworkServer.Spawn(projectile, connectionToClient);
    }

    #endregion

    #region Client
    #endregion
}
