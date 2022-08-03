using System.Collections;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "MovementAbility", menuName = "Abilities/MovementAbility", order = 1)]

    public class MovementAbility : Ability
    {
        public float cooldown = 1f;
        public float movementForce = 100f;
        public MovementCoordinateBase coordBase;
        /// <summary>
        /// Offset of foward direction of whichever coordinate base the movement ability is using, i.e. used like base.forward + forceDirection
        /// </summary>
        public Vector3 forceDirectionOffset = Vector3.zero;

        private MovementActivationTriggerable activator;

        public override void Initialize(GameObject obj)
        {
            activator = obj.GetComponent<MovementActivationTriggerable>();
            activator.movementForce = movementForce;
            activator.forceDirectionOffset = forceDirectionOffset;
            activator.coordBase = coordBase;
        }

        public override void TriggerAbility()
        {
            activator.Activate();
            InvokeOnAbilityActivationSuccess(staminaCost);
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Special movement ability variant for movements with multiple forces
    /// </summary>
    public class DynamicMovementAbility : MovementAbility
    {
        
    }

    public enum MovementCoordinateBase
    {
        Absolute,
        Camera,
        PlayerVel
    }
}