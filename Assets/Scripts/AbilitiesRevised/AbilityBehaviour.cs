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
}
