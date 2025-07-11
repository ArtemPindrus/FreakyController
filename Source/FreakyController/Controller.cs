﻿using FlaxEngine;
using KCC;
using System.Collections.Generic;

namespace FreakyController;

/// <summary>
/// NewController Script.
/// </summary>
public class Controller : Script, IKinematicCharacter
{
    [Serialize]
    [ShowInEditor]
    private InputEvent run = new();

    private List<SpeedDecrease> speedDecreases = new();
    private KinematicCharacterController controller;
    private InputAxis movementForward = new("MovementForward");
    private InputAxis movementRight = new("MovementRight");

    private Locker runningLocker = new();
    private Locker movementLocker = new();

    [Serialize]
    [ShowInEditor]
    private bool applyGravity;

    [Serialize]
    [ShowInEditor]
    [Limit(0,1)]
    private float walkingBackThreshold;

    [Serialize]
    [ShowInEditor]
    private float walkingBackMultiplier;

    /// <summary>
    /// Local movement with 2 axis. X - velocity to the right; Y - velocity forward.
    /// </summary>
    public Vector2 LocalMovementVelocity2D { 
        get {
            var loc = LocalMovementVelocity;

            return new(loc.X, loc.Z);
        }    
    }

    /// <summary>
    /// Horizontal local velocity of controller. Influenced by the player.
    /// </summary>
    [ShowInEditor]
    [ReadOnly]
    public Vector3 LocalMovementVelocity { get; private set; }

    /// <summary>
    /// Vertical local velocity of controller. Is automatic.
    /// </summary>
    [ReadOnly]
    [ShowInEditor]
    public Vector3 GravityVelocity { get; private set; }

    /// <summary>
    /// Max speed of movement in cm/s.
    /// </summary>
    [Serialize]
    [ShowInEditor]
    public float MaxSpeed { get; private set; }

    /// <summary>
    /// Acceleration in cm/s^2.
    /// </summary>
    [Serialize]
    [ShowInEditor]
    public float Acceleration { get; private set; }

    /// <summary>
    /// Multiplier of <see cref="MaxSpeed"/> when Run key is pressed.
    /// </summary>
    [Serialize]
    [ShowInEditor]
    public float RunMultiplier { get; private set; }

    /// <summary>
    /// Height of the controller.
    /// <para>
    /// Automatically adjusts world center.
    /// </para>
    /// </summary>
    [NoSerialize]
    public float Height {
        get => controller.ColliderHeight;
        set {
            var delta = value - controller.ColliderHeight;
            
            controller.ColliderHeight = value;

            controller.SetPosition(controller.Position + new Vector3(0, delta / 2, 0));
        }
    }

    [NoSerialize]
    public Quaternion Orientation {
        get => controller.Orientation;
        private set => controller.SetOrientation(value);
    }

    /// <summary>
    /// Adds decrease of speed to controller and returns reference to its instance.
    /// </summary>
    /// <param name="factor"></param>
    public SpeedDecrease AddSpeedDecrease(float factor) {
        SpeedDecrease s = new(factor);
        speedDecreases.Add(s);

        return s;
    }

    public void Rotate(Quaternion rotation)
    {
        Quaternion newRot = Quaternion.Add(controller.TransientOrientation, rotation);

        Orientation = newRot;
    }

    public void Rotate(float y)
    {
        Quaternion rot = Quaternion.RotationAxis(controller.Transform.Up, y);

        Rotate(rot);
    }

    public void SetHorizontalOrientation(float angle)
    {
        Quaternion or = Quaternion.Euler(new(0, angle, 0));//Quaternion.RotationAxis(controller.Transform.Up, angle);

        Orientation = or;
    }

    public void LockRunning(object @lock, bool toLock) => runningLocker.Lock(@lock, toLock);

    public void LockMovement(object @lock, bool toLock) => movementLocker.Lock(@lock, toLock);

    public void AddWorldAcceleration(Vector3 acceleration) {
        Vector3 delta = acceleration * Time.DeltaTime;

        LocalMovementVelocity += delta; // Not right
    }

    public override void OnAwake()
    {
        controller = Actor.As<KinematicCharacterController>();
        controller.Controller = this;

        Orientation = Actor.Orientation;
    }

    #region Interface
    public void KinematicMoveUpdate(out Vector3 velocity, out Quaternion orientation)
    {
        Accelerate();
        ReduceHorizontalVelocityIfMovingBackwards();
        ApplyGravity();

        velocity = LocalMovementVelocity + GravityVelocity;

        velocity *= Time.DeltaTime; // by default movement is frame-rate dependent, correct with delta time

        orientation = Orientation;
    }

    public Vector3 KinematicGroundProjection(Vector3 velocity, Vector3 gravityEulerNormalized) => 
        controller.GroundTangent(velocity.Normalized) * velocity.Length; // TODO: learn

    public bool KinematicCollisionValid(Collider other) => true;

    public void KinematicCollision(RayCastHit hit) { }

    public void KinematicUnstuckEvent(Collider collider, Vector3 penetrationDirection, float penetrationDistance) { }

    public void KinematicGroundingEvent(GroundState groundingState, RayCastHit? hit)
    {
    }

    public bool KinematicCanAttachToRigidBody(RigidBody rigidBody) => false; // TODO: learn

    public void KinematicAttachedRigidBodyEvent(bool attached, RigidBody rigidBody) { }

    public void KinematicAttachedRigidBodyUpdate(RigidBody rigidBody) { }

    public void KinematicRigidBodyInteraction(RigidBodyInteraction rbInteraction) { }

    public void KinematicPostUpdate() { }
    #endregion

    private void Accelerate()
    {
        Vector3 inputDirection = GetInputDirection();

        if (movementLocker.IsLocked) inputDirection = Vector3.Zero;

        Vector3 targetVelocity = inputDirection * GetTargetSpeed();
        Vector3 velocityDelta = targetVelocity - LocalMovementVelocity;

        float magnitudeDelta = Acceleration * Time.DeltaTime;
        if (velocityDelta.Length > magnitudeDelta)
        {
            velocityDelta = velocityDelta.Normalized * magnitudeDelta;
        }

        LocalMovementVelocity += velocityDelta;
    }

    private void ReduceHorizontalVelocityIfMovingBackwards() {
        float dot = Vector3.Dot(Transform.TransformDirection(LocalMovementVelocity.Normalized), Transform.Forward);

        if (dot < walkingBackThreshold) {
            LocalMovementVelocity *= walkingBackMultiplier;
        }
    }

    private void ApplyGravity()
    {
        if (!applyGravity) return;

        if (!controller.IsGrounded)
        {
            GravityVelocity += Physics.Gravity * Time.DeltaTime;
        }
        else GravityVelocity = new(0, 0, 0);
    }

    private Vector3 GetInputDirection()
    {
        Vector3 inputDirection = new(movementRight.Value, 0, movementForward.Value);
        inputDirection = inputDirection.Normalized;

        return inputDirection;
    }

    private float GetTargetSpeed()
    {
        float targetSpeed = MaxSpeed;
        if (!runningLocker.IsLocked
            && run.State == InputActionState.Pressing)
        {
            targetSpeed *= RunMultiplier;
        }

        foreach (var sd in speedDecreases) {
            if (sd.Enabled) targetSpeed *= sd.Multiplier;
        }

        return targetSpeed * Time.TimeScale;
    }
}

public class SpeedDecrease {
    public float Multiplier { get; }
    public bool Enabled { get; set; }
    
    public SpeedDecrease(float multiplier) {
        Multiplier = multiplier;
    }
}