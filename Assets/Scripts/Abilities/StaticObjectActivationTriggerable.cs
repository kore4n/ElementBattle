using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class StaticObjectActivationTriggerable : NetworkBehaviour
    {
        [SerializeField] Transform spawnParent;

        public void Activate(float lifetime, GameObject gameObject)
        {
            GameObject instance = Instantiate(gameObject, spawnParent);

            NetworkServer.Spawn(instance, connectionToClient);

            Debug.DrawRay(spawnParent.position, spawnParent.position + spawnParent.forward * 10);

            StartCoroutine(DestroyAfterSeconds(instance, lifetime));
        }

        IEnumerator DestroyAfterSeconds(GameObject go, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            NetworkServer.Destroy(gameObject);
        }
    }
}
