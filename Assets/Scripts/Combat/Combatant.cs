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

        [SerializeField] Ability basicAttackAbility;
        [SerializeField] Ability blockAbility;
        [SerializeField] Ability specialAbility;
        [SerializeField] Ability recoveryAbility;

        #region Server

        public override void OnStartServer()
        {
            // Temp initialization
            basicAttackAbility.Initialize(gameObject);
            blockAbility.Initialize(gameObject);
        }

        [Command]
        public void CmdUseAttack()
        {
            if (stamina.GetStamina() < basicAttackAbility.staminaCost) return;

            basicAttackAbility.TriggerAbility();
            stamina.SetStamina(stamina.GetStamina() - basicAttackAbility.staminaCost);
        }

        [Command]
        public void CmdUseBlock()
        {
            if (stamina.GetStamina() < blockAbility.staminaCost) return;

            blockAbility.TriggerAbility();
            stamina.SetStamina(stamina.GetStamina() - blockAbility.staminaCost);
        }

        [Command]
        public void CmdUseSpecial()
        {
            if (stamina.GetStamina() < specialAbility.staminaCost) return;
        
        }

        [Command]
        public void CmdUseRecovery()
        {
            if (stamina.GetStamina() < recoveryAbility.staminaCost) return;
        
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
