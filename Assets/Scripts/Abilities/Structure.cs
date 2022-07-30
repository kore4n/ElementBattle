using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : NetworkBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float moveForce;
    [SerializeField] Vector3 forceDirection;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 spawnOffset = Vector3.zero;
    [SerializeField] float spawnTime = 0.2f;

    Vector3 finalPos;
    float currentSpawnMoveTime;

    public override void OnStartServer()
    {
        finalPos = transform.position;
        currentSpawnMoveTime = spawnTime;
        if (spawnOffset != Vector3.zero) transform.position += spawnOffset;

        if (duration > 0) Invoke(nameof(DestroySelf), duration);

        if (rb == null) return;
        rb.AddForce(moveForce * forceDirection);
    }

    [ServerCallback]
    private void Update()
    {
        if (currentSpawnMoveTime <= 0) return;

        transform.position = Vector3.Lerp(transform.position, finalPos, 1 - currentSpawnMoveTime / spawnTime);
        currentSpawnMoveTime -= Time.deltaTime;
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
