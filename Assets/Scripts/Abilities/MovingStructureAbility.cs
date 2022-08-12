using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "MovingStructureAbility", menuName = "Abilities/MovingStructureAbility", order = 1)]
    public class MovingStructureAbility : StructureAbility
    {
        public int damage;
        public float moveForce;
        public Vector3 moveDirection;
        public bool isBlockable;

        public override void Initialize(GameObject obj)
        {
            base.Initialize(obj);

            Structure structure = structurePrefab.GetComponent<Structure>();
            structure.damage = damage;
            structure.forceDirection = moveDirection;
            structure.moveForce = moveForce;
            structure.isBlockable = isBlockable;
            structure.canMove = true;
        }
    }
}
