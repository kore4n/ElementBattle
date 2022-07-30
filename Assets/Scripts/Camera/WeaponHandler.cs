using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] GameObject cameraHolder = null;
    [SerializeField] GameObject mainCamera = null;
    [SerializeField] GameObject weaponCamera = null;

    void Start()
    {
        if (hasAuthority) { return; }

        // If you have don't authority run this
        //cameraHolder.GetComponent<PlayerAiming>().enabled = false;
        //mainCamera.SetActive(false);
        //weaponCamera.GetComponent<Camera>().enabled = false;
    }
}
