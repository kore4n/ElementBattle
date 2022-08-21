using Game.Combat;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityUser : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] MyCharacterController characterController;
    [SerializeField] AbilityCooldownHandler abilityCooldownHandler;
    [SerializeField] AbilityActivationHandler abilityActivationHandler;
    [SerializeField] AbilityMechanicsHandler abilityMechanicsHandler;
    [SerializeField] Stamina stamina;
    [SerializeField] Transform cameraHolder;

    [Header("Abilities")]
    [SerializeField] Transform abilityHolder;
    [SerializeField] Ability basicAbility;
    [SerializeField] Ability blockAbility;
    [SerializeField] Ability specialAbility;
    [SerializeField] Ability recoveryAbility;

    private AbilityBehaviour basicAbilityBehaviour;
    private AbilityBehaviour blockAbilityBehaviour;
    private AbilityBehaviour specialAbilityBehaviour;
    private AbilityBehaviour recoveryAbilityBehaviour;

    public const string AbilityActivateStartedEventName = "ActivateStarted";
    public const string AbilityActivateCompletedEventName = "ActivateCompleted";
    public const string AbilityOnCooldownActivateEventName = "ActivateOnCooldown";

    public AbilityCooldownHandler AbilityCooldownHandler => abilityCooldownHandler;
    public AbilityActivationHandler AbilityActivationHandler => abilityActivationHandler;
    public AbilityMechanicsHandler AbilityMechanicsHandler => abilityMechanicsHandler;
    public Stamina Stamina => stamina;

    #region Server

    public override void OnStartServer()
    {
        if (basicAbility != null)
        {
            basicAbilityBehaviour = Instantiate(basicAbility.behaviour, abilityHolder);
            basicAbilityBehaviour.Initialize(this);
        }

        if (blockAbility != null)
        {
            blockAbilityBehaviour = Instantiate(blockAbility.behaviour, abilityHolder);
            blockAbilityBehaviour.Initialize(this);
        }

        if (specialAbility != null)
        {
            specialAbilityBehaviour = Instantiate(specialAbility.behaviour, abilityHolder);
            specialAbilityBehaviour.Initialize(this);
        }

        if (recoveryAbility != null)
        {
            recoveryAbilityBehaviour = Instantiate(recoveryAbility.behaviour, abilityHolder);
            recoveryAbilityBehaviour.Initialize(this);
        }

        abilityActivationHandler.ServerOnAbilityActivationCompleted += HandleAbilityActivationCompleted;
    }

    public override void OnStopServer()
    {
        abilityActivationHandler.ServerOnAbilityActivationCompleted -= HandleAbilityActivationCompleted;
    }

    private void HandleAbilityActivationCompleted(int abilityId)
    {
        throw new NotImplementedException();
    }

    [Server]
    private void ActivateAbility(Ability ability, GameObject behaviourObject)
    {
        if (AbilityCooldownHandler.IsOnCooldown(ability.id))
        {
            CustomEvent.Trigger(behaviourObject, AbilityOnCooldownActivateEventName);
            return;
        }
        if (Stamina.GetStamina() < ability.staminaCost) return;

        CustomEvent.Trigger(behaviourObject, AbilityActivateStartedEventName);
    }

    [Command]
    private void ActivateBasicAbilityCommand()
    {
        ActivateAbility(basicAbility, basicAbilityBehaviour.gameObject);
    }

    [Command]
    private void ActivateBlockAbilityCommand()
    {
        ActivateAbility(blockAbility, blockAbilityBehaviour.gameObject);
    }

    [Command]
    private void ActivateSpecialAbilityCommand()
    {
        if (specialAbility == null) return;
    }

    [Command]
    private void ActivateRecoveryAbilityCommand()
    {
        if (recoveryAbility == null) return;
    }

    [Server]
    public void LaunchProjectile(GameObject projectilePrefab)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, cameraHolder.rotation);

        NetworkServer.Spawn(projectileInstance, connectionToClient);
    }

    [Server]
    public void SpawnPlayerObject(GameObject prefab, Transform parent)
    {
        GameObject instance = Instantiate(prefab, parent);

        NetworkServer.Spawn(instance, connectionToClient);
    }

    [Server]
    public void SpawnPlayerObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(prefab, position, rotation, parent);

        NetworkServer.Spawn(instance, connectionToClient);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) return;

        // TEMP: Do proper key bindings later
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ActivateBasicAbilityCommand();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateBlockAbilityCommand();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ActivateSpecialAbilityCommand();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ActivateRecoveryAbilityCommand();
        }
    }

    #endregion
}
