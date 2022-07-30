using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "StructureAbility", menuName = "Abilities/StructureAbility", order = 1)]
    public class StructureAbility : Ability
    {
        public GameObject structurePrefab;

        private StructureSpawnTriggerable spawner;

        public override void Initialize(GameObject obj)
        {
            spawner = obj.GetComponent<StructureSpawnTriggerable>();
            spawner.structurePrefab = structurePrefab;
        }

        public override void TriggerAbility()
        {
            spawner.Spawn();
        }
    }
}