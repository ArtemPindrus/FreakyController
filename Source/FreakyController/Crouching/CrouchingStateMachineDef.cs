using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FreakyController;

public partial class CrouchingStateMachine
{
    private object runningLock = new();

    private SinTween tween;
    private readonly Controller controller;
    private readonly Crouching crouching;
    private readonly SpeedDecrease controllerSpeedDeacrease;

    public CrouchingStateMachine(Controller controller, Crouching crouching) {
        this.controller = controller;
        this.crouching = crouching;


        controllerSpeedDeacrease = controller.AddSpeedDecrease(crouching.CrouchedSpeedMultiplier);

        tween = new(controller.Height, crouching.CrouchedHeight, crouching.CrouchDuration);
        tween.Finished += OnTweenFinished;
    }

    partial void OnRunningLock_Enter()
    {
        controller.LockRunning(runningLock, true);
        controllerSpeedDeacrease.Enabled = true;
    }

    partial void OnRunningLock_Exit()
    {
        controller.LockRunning(runningLock, false);
        controllerSpeedDeacrease.Enabled = false;
    }

    private void OnTweenFinished()
    {
        DispatchEvent(EventId.TweenFinished);
    }

    private void UpdateTween()
    {
        controller.Height = tween.Update(Time.DeltaTime);
    }

    partial void OnIsCrouching_Enter()
    {
        if (tween.Reversed) tween.Reverse();
    }

    partial void OnIsCrouching_Update()
    {
        UpdateTween();
    }

    partial void OnIsUncrouching_Enter()
    {
        if (!tween.Reversed) tween.Reverse();
    }

    partial void OnIsUncrouching_Update()
    {
        UpdateTween();
    }
}
