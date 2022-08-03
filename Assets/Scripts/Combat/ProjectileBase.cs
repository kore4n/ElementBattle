using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBase : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float launchForce = 10;
    [SerializeField] bool isBlockable = true;
    [SerializeField] private float rotationSpeed = 5f;

    // as a %
    [SerializeField] private float damageToDeal = 1f;
    [SerializeField] private float kbMultiplier = 1f;
    [SerializeField] private float destroyAfterSeconds = 5f;

    public Constants.Element element;

    public bool IsBlockable { get { return isBlockable; } }

    private void Start()
    {
        float min = 1f, max = 4f;
        rb.velocity = transform.forward * launchForce;
        rb.angularVelocity = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        //rb.angularVelocity.Normalize();   // Doesn't work for some reason
        rb.angularVelocity = rb.angularVelocity.normalized;

        rb.angularVelocity *= rotationSpeed;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other.gameObject);
        DestroySelf();
    }

    [Server]
    private bool TryDealDamage(GameObject target)
    {
        if (!target.TryGetComponent(out NetworkIdentity networkIdentity)) return false;
        if (!networkIdentity.connectionToClient.identity.TryGetComponent(out FPSPlayer otherPlayer)) return false;
        if (!target.TryGetComponent(out Health health)) return false;
        if (otherPlayer.GetTeam() == GetComponent<NetworkIdentity>().connectionToClient.identity.GetComponent<FPSPlayer>().GetTeam()) return false;

        health.DealDamage((int)damageToDeal);
        return true;
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
