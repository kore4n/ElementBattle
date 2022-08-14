using Fragsurf.Movement;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class ModifySurfCharacterTriggerable : NetworkBehaviour
    {
        [SerializeField] SurfCharacter surfCharacter;

        public SurfCharacter GetSurfCharacter()
        {
            return surfCharacter;
        }

        public void Activate(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
    }
}
