using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;

public class MyCharacterMovement : SurfCharacter
{
    protected override void Start()
    {
        base.Start();

        if (!hasAuthority)
        {
            // Not just camera, also has audio listener we dont want to have active
            viewTransform.gameObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        if (!hasAuthority)
            return;

        base.Update();
    }
}
