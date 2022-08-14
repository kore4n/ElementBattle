using Game.Combat;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] MyCharacterController characterController;
    [SerializeField] AbilityCooldownHandler abilityCooldownHandler;
    [SerializeField] AbilityActivationHandler abilityActivationHandler;
    [SerializeField] Stamina stamina;
    [SerializeField] Transform cameraHolder;

    [Header("Abilities")]
    [SerializeField] AbilityBehaviour basicAbility;
    [SerializeField] AbilityBehaviour blockAbility;
    [SerializeField] AbilityBehaviour specialAbility;
    [SerializeField] AbilityBehaviour recoveryAbility;

    public const string AbilityActivateStartedEventName = "ActivateStarted";
    public const string AbilityActivateCompletedEventName = "ActivateCompleted";

    public AbilityCooldownHandler AbilityCooldownHandler => abilityCooldownHandler;
    public AbilityActivationHandler AbilityActivationHandler => abilityActivationHandler;
    public Stamina Stamina => stamina;

    #region Server

    public override void OnStartServer()
    {
        basicAbility.Initialize(this);

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

    [Command]
    private void ActivateBasicAbilityCommand()
    {
        if (basicAbility == null) return;
        if (stamina.GetStamina() < basicAbility.Ability.staminaCost) return;

        basicAbility.ActivationStarted();
    }

    [Command]
    private void ActivateBlockAbilityCommand()
    {
        if (blockAbility == null) return;

        blockAbility.ActivationStarted();
    }

    [Command]
    private void ActivateSpecialAbilityCommand()
    {
        if (specialAbility == null) return;

        specialAbility.ActivationStarted();
    }

    [Command]
    private void ActivateRecoveryAbilityCommand()
    {
        if (recoveryAbility == null) return;

        recoveryAbility.ActivationStarted();
    }

    [Server]
    public void LaunchProjectile(GameObject projectilePrefab)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, cameraHolder.rotation);

        NetworkServer.Spawn(projectileInstance, connectionToClient);
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
