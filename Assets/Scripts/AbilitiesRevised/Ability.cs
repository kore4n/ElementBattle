using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class Ability : ScriptableObject
{
    [Header("Data")]
    public int id = -1;
    public string abilityName = "New Ability";
    //public Sprite icon;

    [Header("Behaviour")]
    public int staminaCost = 0;
    public float cooldownDuration = 0;
    public float activationDuration = 0;
    public AbilityBehaviour behaviour;
}
