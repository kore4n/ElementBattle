using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public class BlockCollider : NetworkBehaviour
    {
        [SerializeField] float duration = 0.5f;

        Constants.Team ownerTeam;

        public override void OnStartServer()
        {
            Invoke(nameof(DestorySelf), duration);
            ownerTeam = connectionToClient.identity.GetComponent<FPSPlayer>().GetTeam();
        }

        [Server]
        public void DestorySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!TryGetComponent(out ProjectileBase projectile)) return;
            if (projectile.team == ownerTeam || !projectile.IsBlockable) return;

            NetworkServer.Destroy(other.gameObject);
        }
    }
}
