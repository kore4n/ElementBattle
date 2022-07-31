using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FindIdentity : MonoBehaviour
{
    void Start()
    {
        NetworkIdentity[] identities = GetComponentsInChildren<NetworkIdentity>();
        foreach (NetworkIdentity i in identities)
        {
            Debug.Log(i.gameObject.name);
        }
    }
}
