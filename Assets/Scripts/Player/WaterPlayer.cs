using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class WaterPlayer : PlayerBase
    {
        [SerializeField]
        private int hp = 300;
        [SerializeField]
        private float dmgMultiplier = 1f;   // to others
        [SerializeField]
        private float kbMultiplier = 1f;    // knock back multiplier for self
        [SerializeField]
        private float moveMultiplier = 1f;
        [SerializeField]
        private GameObject structure;


        [SerializeField]
        private Element element = Element.water;

        private void Start()
        {
            Health = hp;
            base.dmgMultiplier = dmgMultiplier;
            base.moveMultiplier = moveMultiplier;
            base.element = element;
            base.kbMultiplier = kbMultiplier;
            base.structure = structure;
        }
    }
}