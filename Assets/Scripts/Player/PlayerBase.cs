using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBase : NetworkBehaviour
{
    [SyncVar]
    public string playerName;
    [SyncVar]
    public int dmgSum = 0;

    // as a %
    [Range(0f, 2f)]
    public float dmgMultiplier = 1f;
    [Range(0f, 2f)]
    public float kbMultiplier = 1f;    // knock back multiplier
    [Range(0f, 2f)]
    public float moveMultiplier = 1f;

    public GameObject baseProjectile;

    [SyncVar]
    public Constants.Element element;

    public Color color;


    public void Die()
    {
        // Don't know how to handle this yet
    }

    public virtual void Primary()
    {
        //actionScheduler.SetAbility(actionScheduler.abilities[0]);
    }

    public virtual void Special()
    {

    }

    public virtual void BlockReflect()
    {

    }

    public virtual void Recovery()
    {

    }

    private void Start()
    {
        if (!isServer) { return; }

    }
}
