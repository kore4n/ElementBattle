using UnityEngine;
using System.Collections;

namespace Game.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        public string abilityName = "New Ability";
        public int staminaCost = 0;

        public abstract void Initialize(GameObject obj);
        public abstract void TriggerAbility();
    }
}