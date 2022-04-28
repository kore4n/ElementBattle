using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Player
{
    public class PlayerBase : NetworkBehaviour, IDamageable
    {
        [SyncVar]
        public int health;

        // as a %
        public float dmgMultiplier;
        public float kbMultiplier;    // knock back multiplier
        public float moveMultiplier;

        public GameObject structure;
        public GameObject baseProjectile;
        public GameObject specialProjectile;

        [SyncVar]
        public Constants.Element element;

        public Color color;

        public int Health
        {
            get => health;
            set => health = value;
        }

        public void Damage(int damage)
        {
            Health -= damage;
        }

        private void Start()
        {
            if (!isServer) { return; }

        }
    }
}