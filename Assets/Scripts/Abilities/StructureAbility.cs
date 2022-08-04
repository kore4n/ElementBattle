using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "StructureAbility", menuName = "Abilities/StructureAbility", order = 1)]
    public class StructureAbility : Ability
    {
        public GameObject structurePrefab;
        public int maxStructureInstaces = 1;

        private StructureSpawnTriggerable spawner;

        public override void Initialize(GameObject obj)
        {
            spawner = obj.GetComponent<StructureSpawnTriggerable>();
        }

        public override void TriggerAbility()
        {
            spawner.Spawn(structurePrefab, maxStructureInstaces);
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }
}