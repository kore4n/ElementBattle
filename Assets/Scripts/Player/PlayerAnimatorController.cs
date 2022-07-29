using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : NetworkBehaviour
{
    [SerializeField] private Animator playerBodyAnimator = null;
    [SerializeField] private NetworkAnimator networkWeaponAnimator;
    private Animator weaponAnimator = null;

    [SerializeField] private GameObject[] weaponGameobjects = new GameObject[4];

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer = null;    // Just to turn off your own mesh


    private void Start()
    {
        if (!hasAuthority) { return; }

        skinnedMeshRenderer.enabled = false;

        Constants.Element myElement = NetworkClient.connection.identity.GetComponent<FPSPlayer>().playerElement;

        int myElementIndex = (int)myElement;
        weaponAnimator = weaponGameobjects[myElementIndex].GetComponent<Animator>();

        for (int i = 0; i < 4; i++)
        {
            GameObject curWeapon = weaponGameobjects[i];

            if (curWeapon == null) { continue; }
            if (i == myElementIndex) { continue; }

            curWeapon.SetActive(false);
            networkWeaponAnimator.animator = weaponAnimator;
        }

    }

    private void Update()
    {
        if (!hasAuthority) { return; }

        WeaponAnimation();
        MovementAnimation();
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
            weaponAnimator.SetTrigger("Attack");
        }
        // Raise arms
        else if (Input.GetKeyDown(KeyCode.E))
        {
            weaponAnimator.SetTrigger("Block");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            weaponAnimator.SetTrigger("Recovery");
        }
        // TODO: Specials will have different buttons for now
        else if (Input.GetKeyDown(KeyCode.F))
        {
            weaponAnimator.SetTrigger("SpecialStill");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            weaponAnimator.SetTrigger("SpecialCrouch");
        }
    }

    #endregion
}
