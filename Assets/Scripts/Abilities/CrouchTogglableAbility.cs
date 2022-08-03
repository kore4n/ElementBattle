using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "CrouchTogglableAbility", menuName = "Abilities/CrouchTogglableAbility", order = 1)]
    public class CrouchTogglableAbility : Ability
    {
        private CrouchToggleTriggerable activator;

        public Ability normalAbility;
        public Ability crouchedAbility;

        public override void Initialize(GameObject obj)
        {
            activator = obj.GetComponent<CrouchToggleTriggerable>();
            activator.normalAbility = normalAbility;
            activator.crouchAbility = crouchedAbility;

            normalAbility.Initialize(obj);
            crouchedAbility.Initialize(obj);
        }

        public override void TriggerAbility()
        {
            activator.Activate();
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }
}
