using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using Mirror;
using UnityEditor.Animations;

public class MyCharacterController : NetworkBehaviour
{
    [SerializeField]
    private Animator playerBodyAnimator = null;
    [SerializeField]
    private Animator weaponAnimator = null;

    // Water, earth, fire, air
    [SerializeField] private GameObject[] basicProjectiles = new GameObject[4];
    [SerializeField] private GameObject[] weaponGameobjects = new GameObject[4];

    // Just to turn off your own mesh
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer = null;

    [SerializeField]
    private GameObject cameraShoot = null;

    public GameObject structurePrefab = null;
    public GameObject fakeProjectilePrefab = null;
    public GameObject projectilePrefab = null;

    public GameObject GetCameraHolder()
    {
        return cameraShoot;
    }

    void Start()
    {
        if (!hasAuthority) { return; }

        // Hide local body   TODO: Reenable after testing
        skinnedMeshRenderer.enabled = false;

        Constants.Element myElement = NetworkClient.connection.identity.GetComponent<FPSPlayer>().playerElement;

        int myElementIndex = (int)myElement;
        projectilePrefab = basicProjectiles[myElementIndex];
        weaponAnimator = weaponGameobjects[myElementIndex].GetComponent<Animator>();

        for (int i = 0; i < 4; i++)
        {
            GameObject curWeapon = weaponGameobjects[i];
            
            if (curWeapon == null) { continue; }
            if (i == myElementIndex) { continue; }

            curWeapon.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasAuthority) { return; }


        WeaponAnimation();
        MovementAnimation();

        if (!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        SpawnFakeProjectile();
        SpawnBaseProjectileCommand();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!hasAuthority) { return; }

        // Remove later just for testing
        playerBodyAnimator.SetLookAtWeight(1);
        playerBodyAnimator.SetLookAtPosition(cameraShoot.transform.position + cameraShoot.transform.forward);
    }

    [Client]
    private void SpawnFakeProjectile()
    {
        GameObject projectile = Instantiate(fakeProjectilePrefab, transform.position + cameraShoot.transform.forward, Quaternion.identity);
        projectile.transform.forward = cameraShoot.transform.forward;
        projectile.GetComponent<Rigidbody>().AddForce(cameraShoot.transform.forward * 30, ForceMode.VelocityChange);
    }


    [Command]
    private void SpawnBaseProjectileCommand()
    {
        // Logic
        GameObject projectile = Instantiate(projectilePrefab, transform.position + cameraShoot.transform.forward, Quaternion.identity);
        projectile.transform.forward = cameraShoot.transform.forward;
        projectile.GetComponent<Rigidbody>().AddForce(cameraShoot.transform.forward * 30, ForceMode.VelocityChange);

        NetworkServer.Spawn(projectile, connectionToClient);
    }

    private void MovementAnimation()
    {
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

    void WeaponAnimation()
    {
        // Attack
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weaponAnimator.SetTrigger("Attack");
            Debug.Log("Triggering attack!");
        }
        // Raise arms
        else if (Input.GetKeyDown(KeyCode.E))
        {
            weaponAnimator.SetTrigger("Block");
            Debug.Log("Triggering block!");
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
}
