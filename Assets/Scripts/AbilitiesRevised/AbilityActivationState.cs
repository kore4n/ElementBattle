using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AbilityActivationState
{
    public int AbilityId;
    public float ActivateFinishTime;

    public AbilityActivationState(int abilityId, float activateFinishTime)
    {
        AbilityId = abilityId;
        ActivateFinishTime = activateFinishTime;
    }
}
