using Game.Abilities;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public class Combatant : NetworkBehaviour
    {
        [SerializeField] MyCharacterController characterController;
        [SerializeField] Stamina stamina;

        public AbilitySet abilitySet;

        #region Server

        public override void OnStartServer()
        {
            // Temp initialization
            abilitySet.basicAttackAbility.Initialize(gameObject);
            if (abilitySet.blockAbility != null) abilitySet.blockAbility.Initialize(gameObject);
        }

        [Command]
        public void CmdUseAttack()
        {
            if (stamina.GetStamina() < abilitySet.basicAttackAbility.staminaCost) return;

            abilitySet.basicAttackAbility.TriggerAbility();
            stamina.SetStamina(stamina.GetStamina() - abilitySet.basicAttackAbility.staminaCost);
        }

        [Command]
        public void CmdUseBlock()
        {
            if (stamina.GetStamina() < abilitySet.blockAbility.staminaCost) return;

            abilitySet.blockAbility.TriggerAbility();
            stamina.SetStamina(stamina.GetStamina() - abilitySet.blockAbility.staminaCost);
        }

        [Command]
        public void CmdUseSpecial()
        {
            if (stamina.GetStamina() < abilitySet.specialAbility.staminaCost) return;
        
        }

        [Command]
        public void CmdUseRecovery()
        {
            if (stamina.GetStamina() < abilitySet.recoveryAbility.staminaCost) return;
        
        }

        #endregion

        #region Client

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) return;

            // TEMP: Do proper key bindings later
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                CmdUseAttack();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                CmdUseBlock();
            }
        }
        #endregion
    }
}
