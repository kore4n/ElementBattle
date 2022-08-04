using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "LightningRaycastAbility", menuName = "Abilities/LightningRaycastAbility", order = 1)]
    public class LightningRaycastAbility : Ability
    {
        public GameObject visualModelPrefab;
        public Color delayLineColour;
        public float weaponRange;
        public int weaponDamage;
        public float shootDelay;

        private LightningRaycastShootTriggerable activator;

        public override void Initialize(GameObject obj)
        {
            activator = obj.GetComponent<LightningRaycastShootTriggerable>();
            activator.Initalize();

            activator.raycastLine.material = new Material(Shader.Find("Unlit/Color"));
            activator.raycastLine.material.color = delayLineColour;
        }

        public override void TriggerAbility()
        {
            activator.Activate(weaponDamage, weaponRange, shootDelay, visualModelPrefab);
        }

        public override void TriggerAbilityPreview()
        {
            throw new System.NotImplementedException();
        }
    }
}
