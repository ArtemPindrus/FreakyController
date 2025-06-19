using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FreakyController;

/// <summary>
/// Crouching Script.
/// </summary>
public class Crouching : Script
{
    private CrouchingStateMachine stateMachine;

    [ShowInEditor]
    [ReadOnly]
    private CrouchingStateMachine.StateId State => stateMachine.stateId;

    [Serialize]
    [ShowInEditor]
    private Controller controller;

    [Serialize]
    [ShowInEditor]
    private SphereChecker obstacleChecker;

    [Serialize]
    [ShowInEditor]
    private InputEvent crouchEvent = new();

    [Serialize]
    [ShowInEditor]
    public float CrouchedMultiplier { get; private set; }

    [Serialize]
    [ShowInEditor]
    public float CrouchDuration { get; private set; }

    public float CrouchedHeight => controller.Height * CrouchedMultiplier;

    public override void OnAwake()
    {
        stateMachine = new(controller, this);
        stateMachine.Start();
    }

    public override void OnUpdate()
    {
        stateMachine.DispatchEvent(CrouchingStateMachine.EventId.Update);
    }


    public override void OnEnable()
    {
        crouchEvent.Pressed += OnCrouch_Pressed;
        obstacleChecker.CheckIn += OnObstacleIn;
        obstacleChecker.CheckOut += OnObstacleOut;
    }

    public override void OnDisable()
    {
        crouchEvent.Pressed -= OnCrouch_Pressed;
        obstacleChecker.CheckIn -= OnObstacleIn;
        obstacleChecker.CheckOut -= OnObstacleOut;
    }

    private void OnCrouch_Pressed() => stateMachine.DispatchEvent(CrouchingStateMachine.EventId.CTRL);
    private void OnObstacleIn() => stateMachine.DispatchEvent(CrouchingStateMachine.EventId.ObstacleIn);
    private void OnObstacleOut() => stateMachine.DispatchEvent(CrouchingStateMachine.EventId.ObstacleOut);
}
