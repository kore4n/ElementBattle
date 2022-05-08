using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu]
public class PrimaryFire : Ability
{
    public float recoverVelocity;

    /// <summary>
    /// Do ability.
    /// </summary>
    /// <param name="parent">The gameobject the ability acts on.</param>
    public override void Activate(GameObject parent)
    {
        var projectilePrefab = parent.GetComponent<PlayerBase>().baseProjectile;
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
