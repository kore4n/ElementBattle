using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDestroyAfterSeconds : NetworkBehaviour
{
    [SerializeField] float destroyAfterSeconds;

    private void Start()
    {
        StartCoroutine(DestroyAfterSeconds(destroyAfterSeconds));
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkServer.Destroy(gameObject);
    }
}
