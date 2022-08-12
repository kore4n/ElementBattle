using Fragsurf.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "SurfCharacterAbility", menuName = "Abilities/SurfCharacterAbility", order = 1)]
    public class ModifySurfCharacterAbility : Ability
    {
        // Add any modifiers necessary for future abilities
        public float maxSpeed;
        public float speedChangeDuration = 0;

        private SurfCharacter surfCharacter;

        public override void Initialize(GameObject obj)
        {
            surfCharacter = obj.GetComponent<SurfCharacter>();
        }

        public override void TriggerAbility()
        {
            if (maxSpeed > 0) surfCharacter.moveConfig.maxSpeed = maxSpeed;
        }

        private IEnumerator SpeedChange()
        {
            if (speedChangeDuration <= 0) yield break;

            float originalMaxSpeed = surfCharacter.moveConfig.maxSpeed;
            surfCharacter.moveConfig.maxSpeed = maxSpeed;

            yield return new WaitForSeconds(speedChangeDuration);

            surfCharacter.moveConfig.maxSpeed = originalMaxSpeed;
        }
    }
}
