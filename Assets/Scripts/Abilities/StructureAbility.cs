using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "StructureAbility", menuName = "Abilities/StructureAbility", order = 1)]
    public class StructureAbility : Ability
    {
        public GameObject structurePrefab;
        public int maxStructureInstances = 1;

        private StructureSpawnTriggerable spawner;
        private int structurePrefabId;
        private List<Structure> structureInstanceList;

        private static Dictionary<int, List<Structure>> structureInstanceMap = new();

        public override void Initialize(GameObject obj)
        {
            spawner = obj.GetComponent<StructureSpawnTriggerable>();
            structurePrefabId = structurePrefab.GetInstanceID();
            
            if (!structureInstanceMap.ContainsKey(structurePrefabId))
            {
                structureInstanceMap[structurePrefabId] = new List<Structure>();
            }
            structureInstanceList = structureInstanceMap[structurePrefabId];
        }

        public override void TriggerAbility()
        {
            // This is needed to remove any destroyed structures that may still be in the list
            structureInstanceList.RemoveAll(item => item == null);
            if (structureInstanceList.Count < maxStructureInstances)
            {
                Structure structureInstance = spawner.Spawn(structurePrefab);
                if (structureInstance != null)
                {
                    structureInstanceList.Add(structureInstance);
                }
            }
            else
            {
                Structure firstStructure = structureInstanceList[0];
                structureInstanceList.RemoveAt(0);
                firstStructure.DestroySelf();
            }
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }
}