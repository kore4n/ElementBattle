using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using Player;
using Mirror;

public class MyCharacterController : NetworkBehaviour
{
    [SerializeField]
    private Animator animator;
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



        MovementAnimation();

        if (!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        SpawnBaseProjectileCommand();
    }

    public void SpawnBaseProjectileCommand()
    {
        // Logic
        GameObject projectile = Instantiate(projectilePrefab, transform.position + Vector3.forward, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce(cameraShoot.transform.forward * 30, ForceMode.VelocityChange);
        NetworkServer.Spawn(projectile, connectionToClient);

        Debug.Log("Shooting projectile!");
    }

    [ClientRpc]
    public void SpawnBaseProjectileClientRpc()
    {
        // Visuals
        Debug.Log("Shot projectile!");
    }

    

    void MovementAnimation()
    {
        float dampTime = 0.1f;
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetFloat("Velocity Z", 1f, dampTime, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            animator.SetFloat("Velocity Z", -1f, dampTime, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            animator.SetFloat("Velocity X", -1f, dampTime, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetFloat("Velocity X", 1f, dampTime, Time.deltaTime);
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            animator.SetFloat("Velocity Z", 0f, dampTime, Time.deltaTime);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetFloat("Velocity X", 0f, dampTime, Time.deltaTime);
        }
    }
}
