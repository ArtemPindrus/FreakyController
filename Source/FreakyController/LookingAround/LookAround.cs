using System;
using FlaxEngine;
using KCC;

namespace FreakyController;

/// <summary>
/// LookAround Script.
/// </summary>
public class LookAround : Script
{
    public bool lockCursor;
    public float Sensitivity;
    public float VerticalLimit;

    [Serialize]
    [ShowInEditor]
    private Actor neck;
    [Serialize]
    [ShowInEditor]
    private Controller body;

    private float targetNeckX;
    private float targetBodyY;
    private InputAxis VerticalAxis = new InputAxis("Vertical");
    private InputAxis HorizontalAxis = new InputAxis("Horizontal");
    private CenterAndRange centerAndRange;
    private Locker inputResponseLocker;

    [Serialize]
    [ShowInEditor]
    public Camera Camera { get; private set; }

    public Vector2 LastInputDelta => new(LastInputHorizontalDelta, LastInputVerticalDelta);

    /// <summary>
    /// Last delta to be applied to <see cref="TargetNeckX"/> due to input.
    /// </summary>
    public float LastInputVerticalDelta { get; private set; }

    /// <summary>
    /// Last delta to be applied to <see cref="TargetBodyY"/> due to input.
    /// </summary>
    public float LastInputHorizontalDelta { get; private set; }

    public float TargetNeckX
    {
        get => targetNeckX;
        set
        {
            float clamped = Mathf.Clamp(value, -VerticalLimit, VerticalLimit);

            targetNeckX = clamped;
        }
    }

    public float TargetBodyY { get => targetBodyY; set => targetBodyY = value; }


    public void SetForward(Vector3 forward, out bool isInLimits)
    {
        Matrix initRot = neck.Rotation;

        neck.LookAt(neck.Position + forward.Normalized);
        var afterLookX = neck.LocalEulerAngles.X;
        var afterLookY = neck.LocalEulerAngles.Y;

        isInLimits = afterLookX > -VerticalLimit && afterLookX < VerticalLimit;

        neck.Rotation = initRot;

        TargetNeckX = afterLookX;
        TargetBodyY += afterLookY;
    }

    public void LockInputResponse(object @lock, bool toLock) => inputResponseLocker.Lock(@lock, toLock);

    #region CenterAndRange component API
    public void SetCenterAndRangeTarget(Actor target, float maxAngle)
    {
        centerAndRange.Target = target;
        centerAndRange.MaxAngle = maxAngle;
    }

    public void StopCenterAndRange()
    {
        centerAndRange.Target = null;
        centerAndRange.MaxAngle = 360;
    }
    #endregion

    #region Engine Events
    public override void OnStart()
    {
        centerAndRange = new(Camera, this);
        inputResponseLocker = new();

        if (lockCursor)
        {
            Screen.CursorLock = CursorLockMode.Locked;
            Screen.CursorVisible = false;
        }
    }

    public override void OnUpdate()
    {
        ApplyTargetAngles();
        centerAndRange.Update();
        ApplyTargetAngles();
    }


    public override void OnEnable()
    {
        VerticalAxis.ValueChanged += OnVerticalChanged;
        HorizontalAxis.ValueChanged += OnHorizontalChanged;
    }

    public override void OnDisable()
    {

        VerticalAxis.ValueChanged -= OnVerticalChanged;
        HorizontalAxis.ValueChanged -= OnHorizontalChanged;
    }
    #endregion

    private void ApplyTargetAngles()
    {
        neck.LocalEulerAngles = neck.LocalEulerAngles with { X = TargetNeckX };
        body.SetHorizontalOrientation(TargetBodyY);
    }

    #region Event Handlers
    private void OnVerticalChanged()
    {
        float delta = VerticalAxis.Value * Sensitivity;

        LastInputVerticalDelta = delta;
        if (inputResponseLocker.IsLocked) return;

        TargetNeckX += delta;
    }

    private void OnHorizontalChanged()
    {
        float delta = HorizontalAxis.Value * Sensitivity;

        LastInputHorizontalDelta = delta;

        if (inputResponseLocker.IsLocked) return;
        TargetBodyY += delta;
    }
    #endregion
}
