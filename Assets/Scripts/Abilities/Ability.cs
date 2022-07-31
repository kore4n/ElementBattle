using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        public static IDictionary<string, Func<KeyCode, bool>> ActivationTypes = new Dictionary<string, Func<KeyCode, bool>>()
        {
            { "Key", Input.GetKey },
            { "KeyDown", Input.GetKeyDown },
            { "KeyUp", Input.GetKeyUp }
        };

        /// <summary>
        /// If the given ability is keyUp activated and has a preview when holding keydown (like a structure)
        /// </summary>
        public bool hasPreviewPhase = false;

        public string abilityName = "New Ability";
        public int staminaCost = 0;

        public abstract void Initialize(GameObject obj);
        public abstract void TriggerAbility();
        public abstract void TriggerAbilityPreview();
    }
}