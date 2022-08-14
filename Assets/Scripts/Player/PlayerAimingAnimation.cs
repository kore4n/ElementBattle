using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingAnimation : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject aimTarget;

    void Update()
    {
        Vector3 worldAimTarget;
        worldAimTarget = playerCamera.transform.position + playerCamera.transform.forward * 2f;

        aimTarget.transform.position = worldAimTarget;
    }
}
