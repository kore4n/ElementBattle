using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : NetworkBehaviour
{
    [SerializeField] private Animator playerBodyAnimator = null;
    [SerializeField] private NetworkAnimator networkWeaponAnimator = null;

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer = null;    // Just to turn off your own mesh

    [SerializeField] Animator weaponAnimator = null;
    [SerializeField] GameObject cameraHolder = null;
    [SerializeField] GameObject mainCamera = null;
    [SerializeField] GameObject weaponCamera = null;

    private Vector3 defaultLocalScale;
    private Vector3 defaultLocalPosition;

    #region Subscribe/Unsubscribe
    private void OnEnable()
    {
        GameManager.ClientOnAuthorityGivenBack += ClientHandleAuthorityGivenBack;
    }

    private void OnDisable()
    {
        GameManager.ClientOnAuthorityGivenBack -= ClientHandleAuthorityGivenBack;
    }
    #endregion

    private void Start()
    {
        defaultLocalScale = transform.localScale;
        defaultLocalPosition = transform.localPosition;

        CheckIfMe();
    }
    private void Update()
    {
        if (!hasAuthority) { return; }

        WeaponAnimation();
        MovementAnimation();
    }

    [Client]
    private void CheckIfMe()
    {
        if (!hasAuthority)
        {
            cameraHolder.GetComponent<PlayerAiming>().enabled = false;
            mainCamera.SetActive(false);
            weaponCamera.GetComponent<Camera>().enabled = false;

            // So weapons don't show through players
            int playerLayer = LayerMask.NameToLayer("Player");
            foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>())
            {
                childTransform.gameObject.layer = playerLayer;
            }
            // Make non-local earth weapon smaller
            if (GetComponent<PlayerCharacter>().GetElement() == Constants.Element.Earth)
            {
                var weaponTransform = weaponAnimator.transform;
                weaponTransform.localScale *= 0.2f;
                weaponTransform.localPosition = new Vector3(-0.50f, 0f, 0f);
            }

            return;
        }
        else
        {
            cameraHolder.GetComponent<PlayerAiming>().enabled = true;
            mainCamera.SetActive(true);
            weaponCamera.GetComponent<Camera>().enabled = true;

            int defaultLayer = LayerMask.NameToLayer("Weapon");
            foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>())
            {
                childTransform.gameObject.layer = defaultLayer;
            }
        }


        skinnedMeshRenderer.enabled = false;
    }

    [Client]
    private void ClientHandleAuthorityGivenBack()
    {
        CheckIfMe();
    }


    #region Client Animations
    private void MovementAnimation()
    {
        if (playerBodyAnimator == null)
        {
            Debug.Log("No player body animator assigned.");
            return;
        }

        float dampTime = 0.1f;
        if (Input.GetKey(KeyCode.W))
        {
            playerBodyAnimator.SetFloat("Velocity Z", 1f, dampTime, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerBodyAnimator.SetFloat("Velocity Z", -1f, dampTime, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerBodyAnimator.SetFloat("Velocity X", -1f, dampTime, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerBodyAnimator.SetFloat("Velocity X", 1f, dampTime, Time.deltaTime);
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            playerBodyAnimator.SetFloat("Velocity Z", 0f, dampTime, Time.deltaTime);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            playerBodyAnimator.SetFloat("Velocity X", 0f, dampTime, Time.deltaTime);
        }
    }

    private void WeaponAnimation()
    {
        if (weaponAnimator == null)
        {
            Debug.Log("No player weapon animator assigned.");
            return;
        }

        // Attack
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //weaponAnimator.SetTrigger("Attack");
            networkWeaponAnimator.SetTrigger("Attack");
        }
        // Raise arms
        else if (Input.GetKeyDown(KeyCode.E))
        {
            //weaponAnimator.SetTrigger("Block");
            networkWeaponAnimator.SetTrigger("Block");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            //weaponAnimator.SetTrigger("Recovery");
            networkWeaponAnimator.SetTrigger("Recovery");
        }
        // TODO: Specials will have different buttons for now
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //weaponAnimator.SetTrigger("SpecialStill");
            networkWeaponAnimator.SetTrigger("SpecialStill");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            //weaponAnimator.SetTrigger("SpecialCrouch");
            networkWeaponAnimator.SetTrigger("SpecialCrouch");
        }
    }

    #endregion
}
