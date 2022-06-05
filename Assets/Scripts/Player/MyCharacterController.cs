using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using Mirror;

public class MyCharacterController : NetworkBehaviour
{
    [SerializeField]
    private Animator playerBodyAnimator;
    [SerializeField]
    private Animator armsAnimator;

    public ArmController allArmControllers;

    // Just to turn off your own mesh
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField]
    private GameObject cameraShoot;

    public GameObject structurePrefab;
    public GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Hide local body
        skinnedMeshRenderer.enabled = false;

        // Change arm controller (animations) depending on element
        switch (gameObject.GetComponent<PlayerBase>().element)
        {
            case (Constants.Element.water):
                armsAnimator.runtimeAnimatorController = allArmControllers.waterArms;
                break;
            case (Constants.Element.earth):
                armsAnimator.runtimeAnimatorController = allArmControllers.earthArms;
                break;
            case (Constants.Element.fire):
                armsAnimator.runtimeAnimatorController = allArmControllers.fireArms;
                break;
            case (Constants.Element.air):
                armsAnimator.runtimeAnimatorController = allArmControllers.airArms;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;


        ArmAnimation();
        MovementAnimation();

        if (!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        SpawnBaseProjectileCommand();
    }

    [Command]
    public void SpawnBaseProjectileCommand()
    {
        // Logic
        GameObject projectile = Instantiate(projectilePrefab, transform.position + cameraShoot.transform.forward, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce(cameraShoot.transform.forward * 30, ForceMode.VelocityChange);

        NetworkServer.Spawn(projectile, connectionToClient);
    }

    void MovementAnimation()
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
