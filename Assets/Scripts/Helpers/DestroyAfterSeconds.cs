using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : NetworkBehaviour
{
    [SerializeField] float destroyAfterSeconds;

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
}
