using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityActivationHandler : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleAbilityActivationStateChanged))]
    private AbilityActivationState abilityActivationState = new(-1, 0);

    public event Action<AbilityActivationState> ClientOnAbilityActivationStarted;
    public event Action ClientOnAbilityActivationCompleted;
    public event Action<int> ServerOnAbilityActivationCompleted;

    public bool IsActivating => abilityActivationState.AbilityId != -1;
    public AbilityActivationState AbilityActivationState => abilityActivationState;

    private void HandleAbilityActivationStateChanged(AbilityActivationState oldState, AbilityActivationState newState)
    {
        if (newState.AbilityId == -1)
        {
            ClientOnAbilityActivationCompleted?.Invoke();
            return;
        }

        ClientOnAbilityActivationStarted?.Invoke(newState);
    }

    [Server]
    public void Activate(Ability ability)
    {
        if (IsActivating) return;

        abilityActivationState = new(ability.id, (float)NetworkTime.time + ability.activationDuration);
    }
}
