using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Player
{
    public class PlayerBase : NetworkBehaviour, IDamageable
    {
        protected int health;

        // as a %
        protected float dmgMultiplier;
        protected float kbMultiplier;    // knock back multiplier
        protected float moveMultiplier;
        
        protected GameObject structure;

        protected Element element;

        public int Health
        {
            get => health;
            set => health = value;
        }

        protected enum Element
        {
            water,
            earth,
            fire,
            air
        }

        public void Damage(int damage)
        {
            Health -= damage;
        }
    }
}