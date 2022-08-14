using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    /// <summary>
    /// Useful for objects that can be responsible for their own behaviour and only need to be spawned
    /// </summary>
    [CreateAssetMenu(fileName = "StaticObjectAbility", menuName = "Abilities/StaticObjectAbility", order = 1)]
    public class StaticObjectAbility : Ability
    {
        public float lifetime = -1;
        public GameObject objectPrefab;

        StaticObjectActivationTriggerable activator;

        public override void Initialize(GameObject obj)
        {
            activator = obj.GetComponent<StaticObjectActivationTriggerable>();
        }

        public override void TriggerAbility()
        {
            activator.Activate(lifetime, objectPrefab);
        }
    }
}
