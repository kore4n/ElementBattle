using Fragsurf.Movement;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class MovementActivationTriggerable : NetworkBehaviour
    {
        [HideInInspector] public float movementForce;
        [HideInInspector] public Vector3 forceDirectionOffset;
        [HideInInspector] public MovementCoordinateBase coordBase;

        [SerializeField] Transform cameraHolder;
        [SerializeField] SurfCharacter surfCharacter;

        [Server]
        public void Activate()
        {
            switch (coordBase)
            {
                case MovementCoordinateBase.Absolute:
                    MoveTowards(forceDirectionOffset, movementForce);
                    break;
                case MovementCoordinateBase.Camera:
                    MoveTowards(cameraHolder.forward + cameraHolder.rotation * forceDirectionOffset, movementForce);
                    break;
                case MovementCoordinateBase.PlayerVel:
                    MoveTowards(surfCharacter.baseVelocity + Quaternion.FromToRotation(Vector3.forward, surfCharacter.baseVelocity) * forceDirectionOffset, movementForce);
                    break;
            }
        }

        [Server]
        private void MoveTowards(Vector3 dir, float forceMultiplier)
        {
            surfCharacter._controller.forceMultiplier = forceMultiplier;
            surfCharacter._controller.jumpDirection = dir;
            surfCharacter.moveData.wishJumpForward = true;
        }

        private void OnDrawGizmos()
        {
            // Camera forward vector
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (cameraHolder.forward + forceDirectionOffset).normalized);

            // Velocity vector
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + (surfCharacter.baseVelocity + forceDirectionOffset).normalized);
        }
    }
}
