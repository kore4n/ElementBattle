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

    public override void OnStartServer()
    {
        if (spawnOffset != Vector3.zero)
        {
            Vector3 finalPos = transform.position;
            transform.position += spawnOffset;
            StartCoroutine(MoveOverTime(transform.position, finalPos, spawnTime));
        }

        if (duration > 0) Invoke(nameof(DestroySelf), duration);

        if (rb == null) return;
        rb.AddForce(moveForce * forceDirection);
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
    }

    [Server]
    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
