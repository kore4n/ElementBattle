using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProjectileBase : NetworkBehaviour
{
    // as a %
    public float dmg;
    public float kbMultiplier;    // knock back multiplier

    public Constants.Element element;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) { return; }


    }
}
