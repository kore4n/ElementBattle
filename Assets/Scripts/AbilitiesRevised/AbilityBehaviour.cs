using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Ability ability;

    Fighter fighter;

    public Ability Ability => ability;
    public Fighter Fighter => fighter;

    public void Initialize(Fighter fighter)
    {
        this.fighter = fighter;
    }

    public void ActivationStarted()
    {
        if (fighter.AbilityCooldownHandler.IsOnCooldown(ability.id)) return;
        if (fighter.Stamina.GetStamina() < ability.staminaCost) return;

        CustomEvent.Trigger(gameObject, Fighter.AbilityActivateStartedEventName);
    }

    public void ActivationCompleted()
    {
        CustomEvent.Trigger(gameObject, Fighter.AbilityActivateCompletedEventName);
    }
}
