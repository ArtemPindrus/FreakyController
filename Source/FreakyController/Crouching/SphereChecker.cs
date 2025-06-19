using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FreakyController;

/// <summary>
/// SphereOverlaper Script.
/// </summary>
public class SphereChecker : Script
{
#nullable enable
    public event Action? CheckIn;
    public event Action? CheckOut;
#nullable disable

    [Serialize]
    [ShowInEditor]
    private Vector3 localCenter;

    [Serialize]
    [ShowInEditor]
    private float radius;

    [Serialize]
    [ShowInEditor]
    private LayersMask hitMask;

    [Serialize]
    [ShowInEditor]
    private bool hitTriggers;

    /// <summary>
    /// Whether checker hit the last frame.
    /// </summary>
    [NoSerialize]
    [ReadOnly]
    public bool IsHit;

    public Vector3 WorldCenter => Transform.TransformPoint(localCenter);

#nullable enable
    public override void OnUpdate()
    {
        bool isHitTemp = Physics.CheckSphere(WorldCenter, radius, hitMask, hitTriggers);

        if (!IsHit && isHitTemp) CheckIn?.Invoke();
        else if (IsHit && !isHitTemp) CheckOut?.Invoke();

        IsHit = isHitTemp;
    }

    public override void OnDebugDraw()
    {
        BoundingSphere s = new(WorldCenter, radius);
        Color color = IsHit ? Color.Green : Color.Red;

        DebugDraw.DrawSphere(s, color);
    }
}
