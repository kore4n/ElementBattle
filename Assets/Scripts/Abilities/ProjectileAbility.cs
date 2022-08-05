using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "ProjectileAbility", menuName = "Abilities/ProjectileAbility", order = 1)]
    public class ProjectileAbility : Ability
    {
        public GameObject projectilePrefab;

        private ProjectileShootTriggerable launcher;

        public override void Initialize(GameObject obj)
        {
            launcher = obj.GetComponent<ProjectileShootTriggerable>();
        }

        public override void TriggerAbility()
        {
            launcher.Launch(projectilePrefab);
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }
}
