using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use for players and structures.
/// </summary>
public interface IDamageable
{
    public int DmgSum { get; set; }

    /// <summary>
    /// Apply the amount of damage.
    /// </summary>
    /// <returns></returns>
    public void Damage(int damage);

    /// <summary>
    /// Respawn afterwards if player.
    /// </summary>
    public void Die();
}
