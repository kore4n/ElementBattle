using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Ability ability;

    public Ability Ability => ability;
    public AbilityUser AbilityUser { get; private set; }

    public virtual void Initialize(AbilityUser abilityUser)
    {
        AbilityUser = abilityUser;
    }

    public void ActivationStarted()
    {
        if (AbilityUser.AbilityCooldownHandler.IsOnCooldown(ability.id)) return;
        if (AbilityUser.Stamina.GetStamina() < ability.staminaCost) return;

        CustomEvent.Trigger(gameObject, AbilityUser.AbilityActivateStartedEventName);
    }

    public void ActivationCompleted()
    {
        CustomEvent.Trigger(gameObject, AbilityUser.AbilityActivateCompletedEventName);
    }
}
