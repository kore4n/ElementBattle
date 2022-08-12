using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Structure : NetworkBehaviour
{
    [HideInInspector] public float moveForce;
    [HideInInspector] public Vector3 forceDirection;
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public int damage;
    [HideInInspector] public float knockbackForce;
    [HideInInspector] public bool isBlockable = true;

    [SerializeField] float lifetime;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 spawnOffset = Vector3.zero;
    [SerializeField] float spawnTime = 0.2f;

    public override void OnStartServer()
    {
        if (spawnOffset != Vector3.zero)
        {
            Vector3 finalPos = transform.position;
            transform.position += spawnOffset;
            StartCoroutine(MoveOverTime(transform.position, finalPos, spawnTime));
        }

        if (lifetime > 0) Invoke(nameof(DestroySelf), lifetime);

        if (!canMove || spawnOffset != Vector3.zero) return;
        rb.velocity = transform.localToWorldMatrix * forceDirection * moveForce;
    }
    IEnumerator MoveOverTime(Vector3 originalLocation, Vector3 finalLocation, float time)
    {
        float currentTime = 0.0f;

        do
        {
            transform.position = Vector3.Lerp(originalLocation, finalLocation, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

        if (canMove)
        {
            rb.velocity = (transform.localToWorldMatrix * forceDirection).normalized * moveForce;
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!canMove) return;
        if (!isBlockable && other.TryGetComponent(out Structure structure))
        {
            structure.DestroySelf();
            return;
        }
        if (!TryGetComponent(out Health health)) return;

        health.DealDamage(damage);
    }

    [Server]
    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
