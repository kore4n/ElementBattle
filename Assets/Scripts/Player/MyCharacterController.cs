using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using Mirror;

public class MyCharacterController : NetworkBehaviour
{
    // Water, earth, fire, air
    [SerializeField] private GameObject[] basicProjectiles = new GameObject[4];

    [SerializeField] private GameObject cameraHolder = null;

    public GameObject structurePrefab = null;
    public GameObject projectilePrefab = null;
    [SerializeField] GameObject testProjectile = null;

    public GameObject GetCameraHolder()
    {
        return cameraHolder;
    }

    void Start()
    {
        // Not just camera, also has audio listener we dont want to have active
        //cameraHolder.GetComponent<PlayerAiming>().enabled = false;

        if (!hasAuthority) { return; }

        Constants.Element myElement = NetworkClient.connection.identity.GetComponent<FPSPlayer>().playerElement;

        int myElementIndex = (int)myElement;
        projectilePrefab = basicProjectiles[myElementIndex];
    }

    void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Instantiate(testProjectile, transform.position, Quaternion.identity);
            Debug.Log(Cursor.lockState);
        }

        if (!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        SpawnBaseProjectileCommand();
    }

    #region Server

    [Command]
    private void SpawnBaseProjectileCommand()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.transform.forward = cameraHolder.transform.forward;

        NetworkServer.Spawn(projectile, connectionToClient);
    }

    #endregion

    #region Client
    #endregion
}
