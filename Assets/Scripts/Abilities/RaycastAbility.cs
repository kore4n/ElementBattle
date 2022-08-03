using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "RaycastAbility", menuName = "Abilities/RaycastAbility", order = 1)]
    public class RaycastAbility : Ability
    {
        public GameObject visualModelPrefab;
        public float weaponRange;
        public int weaponDamage;

        private RaycastShootTriggerable activator;

        public override void Initialize(GameObject obj)
        {
            activator = obj.GetComponent<RaycastShootTriggerable>();
            activator.visualModelPrefab = visualModelPrefab;
            activator.range = weaponRange;
            activator.damage = weaponDamage;
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
