using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class RaycastVisual : NetworkBehaviour
    {
        public float scalePerMeter = 1f;
        [SerializeField] float lingerTime = 0.5f;

        [HideInInspector] public float maxSquaredDistance;
        [HideInInspector] public Vector3 startingPosition;

        public override void OnStartServer()
        {
            Destroy(gameObject, lingerTime);
        }
    }
}
