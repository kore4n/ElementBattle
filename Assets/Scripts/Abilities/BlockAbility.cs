using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "BlockAbility", menuName = "Abilities/BlockAbility", order = 1)]
    public class BlockAbility : Ability
    {
        public GameObject blockAreaPrefab;

        private BlockSpawnTriggerable spawner;

        public override void Initialize(GameObject obj)
        {
            spawner = obj.GetComponent<BlockSpawnTriggerable>();
            spawner.blockAreaPrefab = blockAreaPrefab;
        }

        public override void TriggerAbility()
        {
            spawner.Spawn();
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }
}
