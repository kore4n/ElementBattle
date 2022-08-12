using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class AirBlast : NetworkBehaviour
    {
        public Vector3 forceDirection;
        public float pushForce;

        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out NetworkIdentity networkIdentity)) return;
            if (!networkIdentity.connectionToClient.identity.TryGetComponent(out FPSPlayer otherPlayer)) return;
            if (otherPlayer.GetTeam() == GetComponent<NetworkIdentity>().connectionToClient.identity.GetComponent<FPSPlayer>().GetTeam()) return;
            if (!TryGetComponent(out Rigidbody rigidbody)) return;

            rigidbody.AddForce(transform.localToWorldMatrix * forceDirection.normalized * pushForce * Time.deltaTime);
        }
    }
}
