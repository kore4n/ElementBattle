using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProjectileBase : NetworkBehaviour
{
    // as a %
    [SerializeField] private float damageToDeal = 1f;
    [SerializeField] private float kbMultiplier = 1f;
    [SerializeField] private float destroyAfterSeconds = 5f;

    public Constants.Element element;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision == null) { return; }


    //}

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        GameObject playerCharacter = other.transform.parent.gameObject;
        if (playerCharacter.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) { return; }
        }


        if (playerCharacter.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage((int)damageToDeal);
        }

        if (other.gameObject.layer == 0) { DestroySelf(); }

        Debug.Log(playerCharacter.name);
        Debug.Log(playerCharacter.GetComponent<NetworkIdentity>());

        DestroySelf();  // Called when hit anything that doesn't belong to self
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
