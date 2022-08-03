using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    public class RaycastShootTriggerable : NetworkBehaviour
    {
        [HideInInspector] public GameObject visualModelPrefab;
        [HideInInspector] public float range;
        [HideInInspector] public int damage;

        [SerializeField] Transform cameraTransform;
        [SerializeField] Transform shootOrigin;
        [SerializeField] LayerMask targetLayerMask;

        private Vector3 endPoint;

        [Server]
        public void Activate()
        {
            //Create a vector at the center of our camera's near clip plane.
            Vector3 rayOrigin = shootOrigin.position;

            if (Physics.Raycast(rayOrigin, cameraTransform.forward, out RaycastHit hit, range, targetLayerMask))
            {
                endPoint = hit.point;

                if (hit.collider.TryGetComponent(out Health health))
                {
                    health.DealDamage(damage);
                }
            }
            else
            {
                endPoint = cameraTransform.forward * range;
            }

            Debug.DrawRay(rayOrigin, cameraTransform.forward * range, Color.green, 1f);

            GameObject visualModelInstance = Instantiate(visualModelPrefab, shootOrigin.position, Quaternion.FromToRotation(Vector3.forward, endPoint - rayOrigin));
            RaycastVisual projectile = visualModelInstance.GetComponent<RaycastVisual>();

            int distance = Mathf.RoundToInt((endPoint - rayOrigin).magnitude);

            visualModelInstance.transform.localScale = new Vector3(
                visualModelInstance.transform.localScale.x,
                projectile.scalePerMeter * distance,
                visualModelInstance.transform.localScale.z
                );

            NetworkServer.Spawn(visualModelInstance, connectionToClient);
        }
    }
}
