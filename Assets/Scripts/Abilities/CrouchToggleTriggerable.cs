using Fragsurf.Movement;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [RequireComponent(typeof(SurfCharacter))]
    public class CrouchToggleTriggerable : NetworkBehaviour
    {
        [SerializeField] SurfCharacter surfCharacter;

        [HideInInspector] public Ability normalAbility;
        [HideInInspector] public Ability crouchAbility;

        public void Activate()
        {
            if (surfCharacter.moveData.crouching)
            {
                crouchAbility.TriggerAbility();
                return;
            }

            normalAbility.TriggerAbility();
        }
    }
}
