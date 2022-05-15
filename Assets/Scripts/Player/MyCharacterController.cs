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
            Animator animator = GetComponent<Animator>();

            return;
        }

        // Hide local body
        skinnedMeshRenderer.enabled = false;


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
            armsAnimator.SetTrigger("RaiseUp");
        }
    }
}
