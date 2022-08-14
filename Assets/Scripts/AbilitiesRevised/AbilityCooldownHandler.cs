using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCooldownHandler : NetworkBehaviour
{
    readonly SyncList<AbilityCooldownState> abilitiesOnCooldown = new SyncList<AbilityCooldownState>();

    public event Action<AbilityCooldownState> OnAbilityCooldownStarted;

    #region Server

    [ServerCallback]
    private void Update()
    {
        int i;
        for (i = abilitiesOnCooldown.Count - 1; i >= 0; i--)
        {
            if (NetworkTime.time >= abilitiesOnCooldown[i].AbilityReadyTime)
            {
                abilitiesOnCooldown.RemoveAt(i);
            }
        }
    }

    [Server]
    public void PutOnCooldown(Ability ability)
    {
        var cooldownState = new AbilityCooldownState(ability.id, (float)NetworkTime.time + ability.cooldownDuration);
        abilitiesOnCooldown.Add(cooldownState);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!hasAuthority) return;

        abilitiesOnCooldown.Callback += HandleAbilitiesOnCooldownChanged;
    }

    #endregion

    public bool IsOnCooldown(int abilityId)
    {
        return abilitiesOnCooldown.FindIndex(x => x.AbilityId == abilityId) != -1;
    }

    public AbilityCooldownState GetCooldownState(int abilityId)
    {
        return abilitiesOnCooldown.Find(x => x.AbilityId == abilityId);
    }

    private void HandleAbilitiesOnCooldownChanged(SyncList<AbilityCooldownState>.Operation op, int index, AbilityCooldownState oldState, AbilityCooldownState newState)
    {
        switch (op)
        {
            case SyncList<AbilityCooldownState>.Operation.OP_ADD:
                OnAbilityCooldownStarted?.Invoke(newState);
                break;
        }
    }
}
