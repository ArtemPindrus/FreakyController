using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FreakyController;

/// <summary>
/// PreserveDisplacement Script.
/// </summary>
public class PreserveDisplacement : Script
{
    [Serialize]
    [ShowInEditor]
    private Controller controller;

    private float initialHeight;
    private Vector3 initialLocalDisplacement;

    public override void OnAwake()
    {
        initialLocalDisplacement = Actor.LocalPosition;
        initialHeight = controller.Height;
    }

    public override void OnUpdate() {
        Actor.LocalPosition = controller.Height * initialLocalDisplacement / initialHeight;
    }
}
