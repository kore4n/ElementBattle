using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "AbilitySet", menuName ="Abilities/AbilitySet", order = 1)]
    public class AbilitySet : ScriptableObject
    {
        public Ability basicAttackAbility;
        public Ability blockAbility;
        public Ability specialAbility;
        public Ability recoveryAbility;
    }
}
