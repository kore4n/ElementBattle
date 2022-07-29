using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "BlockAbility", menuName = "Abilities/BlockAbility", order = 1)]
    public class BlockAbility : Ability
    {
        public GameObject shieldPrefab;

        private BlockSpawnTriggerable spawner;

        public override void Initialize(GameObject obj)
        {
            spawner = obj.GetComponent<BlockSpawnTriggerable>();
            spawner.shieldPrefab = shieldPrefab;
        }

        public override void TriggerAbility()
        {
            spawner.Spawn();
        }
    }
}
