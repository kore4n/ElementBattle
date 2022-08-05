using Fragsurf.Movement;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Abilities
{
    [RequireComponent(typeof(LineRenderer))]
    public class LightningRaycastShootTriggerable : NetworkBehaviour
    {
        [SerializeField] Camera playerCamera;
        [SerializeField] Transform shootOrigin;
        [SerializeField] LayerMask targetLayerMask;
        [SerializeField] SurfCharacter surfCharacter;

        [HideInInspector] public LineRenderer raycastLine;

        private float originalWalkSpeed;
        private bool shooting = false;
        private float range;

        public void Initalize()
        {
            raycastLine = GetComponent<LineRenderer>();
            originalWalkSpeed = surfCharacter.movementConfig.walkSpeed;
        }

        [ServerCallback]
        private void Update()
        {
            if (!shooting) return;

            raycastLine.SetPosition(0, transform.position);
            raycastLine.SetPosition(1, transform.position + playerCamera.transform.forward * range);
        }

        [Server]
        public void Activate(int damage, float range, float delayTime, GameObject visualModelPrefab)
        {
            this.range = range;
            shooting = true;

            Update();
            RpcSetRaycastLineEnabled(true);
            surfCharacter.movementConfig.walkSpeed = 0.1f;

            StartCoroutine(DelayShootRaycast(damage, range, delayTime, visualModelPrefab));
        }

        IEnumerator DelayShootRaycast(int damage, float range, float delayTime, GameObject visualModelPrefab)
        {
            yield return new WaitForSeconds(delayTime);

            surfCharacter.movementConfig.walkSpeed = originalWalkSpeed;
            RpcSetRaycastLineEnabled(false);

            Vector3 hitpoint;
            if (Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit hit, range, targetLayerMask))
            {
                hitpoint = hit.point;
                if (hit.collider.TryGetComponent(out Health health))
                {
                    health.DealDamage(damage);
                }
            }
            else
            {
                hitpoint = transform.position + playerCamera.transform.forward * range;
            }
            GameObject lightningInstance = Instantiate(visualModelPrefab, playerCamera.transform.position, playerCamera.transform.rotation);
            float hitDist = (hitpoint - transform.position).magnitude;

            NetworkServer.Spawn(lightningInstance, connectionToClient);

            RpcActivateVisual(lightningInstance, hitDist);
            //visual = lightningInstance.GetComponentInChildren<VisualEffect>();
            //visual.SetFloat("LightningLength", hitDist);
            //visual.SetFloat("ImpactOffset", -hitDist / 10);
            //visual.enabled = true;
            //visual.Play();
            //StartCoroutine(DisableAfterSeconds(visual, 2));

            shooting = false;
        }

        IEnumerator DisableAfterSeconds(VisualEffect component, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            component.enabled = false;
        }

        [ClientRpc]
        private void RpcSetRaycastLineEnabled(bool enabled)
        {
            raycastLine.enabled = enabled;
        }

        [ClientRpc]
        private void RpcActivateVisual(GameObject lightningInstance, float hitDist)
        {
            VisualEffect visual = lightningInstance.GetComponentInChildren<VisualEffect>();
            visual.SetFloat("LightningLength", hitDist);
            visual.SetFloat("ImpactOffset", -hitDist / 10);
            visual.enabled = true;
            visual.Play();
            StartCoroutine(DisableAfterSeconds(visual, 2));
        }
    }
}
