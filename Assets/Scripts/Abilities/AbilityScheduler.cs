using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScheduler : MonoBehaviour
{
    public Ability currentAbility;

    /// <summary>
    /// Replace current ability with this one.
    /// </summary>
    public void SetAbility(Ability ability)
    {
        CancelAbility();

        // 
    }

    private void CancelAbility()
    {
        currentAbility = null;
    }
}
