using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProjectileBase : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float launchForce = 10;

    // as a %
    [SerializeField] private float damageToDeal = 1f;
    [SerializeField] private float kbMultiplier = 1f;
    [SerializeField] private float destroyAfterSeconds = 5f;

    public Constants.Element element;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    //[ServerCallback]
    //private void OnTriggerEnter(Collider other)
    //{
    //    GameObject playerCharacter = other.transform.parent.gameObject;
    //    if (playerCharacter.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
    //    {
    //        if (networkIdentity.connectionToClient == connectionToClient) { return; }
    //    }


    //    if (playerCharacter.TryGetComponent<Health>(out Health health))
    //    {
    //        health.DealDamage((int)damageToDeal);
    //    }

    //    if (other.gameObject.layer == 0) { DestroySelf(); } // TODO: Doesnt work

    //    //Debug.Log(playerCharacter.name);
    //    //Debug.Log(playerCharacter.GetComponent<NetworkIdentity>());

    //    DestroySelf();  // Called when hit anything that doesn't belong to self
    //}

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
