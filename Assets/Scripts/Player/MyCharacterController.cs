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
    private Animator armsAnimator = null;

    // Water, earth, fire, air
    [SerializeField] private GameObject[] basicProjectiles = new GameObject[4];
    [SerializeField] private RuntimeAnimatorController[] armAnimators = new RuntimeAnimatorController[4];

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
        if (!hasAuthority)
        {
            return;
        }

        // Hide local body
        skinnedMeshRenderer.enabled = false;

        Constants.Element myElement = NetworkClient.connection.identity.GetComponent<FPSPlayer>().playerElement;

        projectilePrefab = basicProjectiles[(int)myElement];
        armsAnimator.runtimeAnimatorController = armAnimators[(int)myElement];
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;


        ArmAnimation();
        MovementAnimation();

        if (!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        SpawnFakeProjectile();
        SpawnBaseProjectileCommand();
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

    void ArmAnimation()
    {
        // Attack
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            armsAnimator.SetTrigger("Attack");
        }
        // Raise arms
        else if (Input.GetKeyDown(KeyCode.E))
        {
            armsAnimator.SetTrigger("Block");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            armsAnimator.SetTrigger("Recovery");
        }
        // TODO: Specials will have different buttons for now
        else if (Input.GetKeyDown(KeyCode.F))
        {
            armsAnimator.SetTrigger("SpecialStill");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            armsAnimator.SetTrigger("SpecialCrouch");
        }
    }
}
