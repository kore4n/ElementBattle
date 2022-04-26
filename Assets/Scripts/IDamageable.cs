using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use for players and structures.
/// </summary>
public interface IDamageable
{
    int Health { get; set; }

    /// <summary>
    /// Apply the amount of damage.
    /// </summary>
    /// <returns></returns>
    void Damage(int damage);
}
