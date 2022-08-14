using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AbilityCooldownState
{
    public int AbilityId;
    public float AbilityReadyTime;

    public AbilityCooldownState(int abilityId, float abilityReadyTime)
    {
        AbilityId = abilityId;
        AbilityReadyTime = abilityReadyTime;
    }
}
