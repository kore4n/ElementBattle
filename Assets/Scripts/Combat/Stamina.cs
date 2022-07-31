using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public class Stamina : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleStaminaUpdated))]
        int stamina = 9;

        [SerializeField] int maxStamina = 9;
        [SerializeField] float staminaRegenerationInterval = 1f;
        
        float staminaRegenerationCooldown = 0;

        public event Action<int> ClientOnStaminaUpdated;

        #region Getters/Setters

        public int GetStamina()
        {
            return stamina;
        }

        public void SetStamina(int newStamina)
        {
            stamina = newStamina;
        }

        #endregion

        [ServerCallback]
        void Update()
        {
            UpdateStamina();
        }

        [Server]
        private void UpdateStamina()
        {
            if (stamina == maxStamina) return;
            staminaRegenerationCooldown -= Time.deltaTime;

            if (staminaRegenerationCooldown > 0) return;

            stamina++;
            staminaRegenerationCooldown = staminaRegenerationInterval;
        }

        private void HandleStaminaUpdated(int oldStamina, int newStamina)
        {
            ClientOnStaminaUpdated?.Invoke(newStamina);
        }
    }
}
