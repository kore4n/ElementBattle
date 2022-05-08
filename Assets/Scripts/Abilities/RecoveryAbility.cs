using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RecoveryAbility : Ability
{
    public float recoverVelocity;
    
    /// <summary>
    /// Do ability.
    /// </summary>
    /// <param name="parent">The gameobject the ability acts on.</param>
    public override void Activate(GameObject parent)
    {
        parent.transform.position = parent.transform.position + Vector3.up * 5f;
        Debug.Log("Ability activated!");
    }

    /// <summary>
    /// What happens when ability is over.
    /// </summary>
    /// <param name="parent"></param>
    public override void BeginCooldown(GameObject parent)
    {
        base.BeginCooldown(parent);
    }
}
