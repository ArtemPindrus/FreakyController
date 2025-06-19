using FlaxEngine;
using KCC;

namespace FreakyController;

/// <summary>
/// NewController Script.
/// </summary>
public class Controller : Script, IKinematicCharacter
{
    [Serialize]
    [ShowInEditor]
    private InputEvent run = new();

    private KinematicCharacterController controller;
    private InputAxis movementForward = new("MovementForward");
    private InputAxis movementRight = new("MovementRight");
    private Locker runningLocker = new();

    /// <summary>
    /// Horizontal velocity of controller. Influenced by the player.
    /// </summary>
    [ShowInEditor]
    [ReadOnly]
    private Vector3 movementVelocity;

    [ReadOnly]
    [ShowInEditor]
    private Vector3 gravityVelocity;

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

    [NoSerialize]
    public float Height
    {
        get => controller.ColliderHeight; set => controller.ColliderHeight = value;
    }

    [ShowInEditor]
    [ReadOnly]
    public GroundState GroundingState { get; private set; }

    public void Rotate(Quaternion rotation)
    {
        Quaternion newRot = Quaternion.Add(controller.TransientOrientation, rotation);

        controller.SetOrientation(newRot);
    }

    public void Rotate(float y)
    {
        Quaternion rot = Quaternion.RotationAxis(controller.Transform.Up, y);

        Rotate(rot);
    }

    public void SetHorizontalOrientation(float angle)
    {
        Quaternion or = Quaternion.Euler(new(0, angle, 0));//Quaternion.RotationAxis(controller.Transform.Up, angle);

        controller.SetOrientation(or);
    }

    public void LockRunning(object @lock, bool toLock) => runningLocker.Lock(@lock, toLock);

    public override void OnAwake()
    {
        controller = Actor.As<KinematicCharacterController>();
        controller.Controller = this;
    }

    #region Interface
    public void KinematicMoveUpdate(out Vector3 velocity, out Quaternion orientation)
    {
        Accelerate();
        ApplyGravity();
        velocity = movementVelocity + gravityVelocity;

        orientation = Quaternion.Identity;
    }

    public Vector3 KinematicGroundProjection(Vector3 velocity, Vector3 gravityEulerNormalized) =>
        controller.GroundTangent(velocity.Normalized) * velocity.Length; // TODO: learn

    public bool KinematicCollisionValid(Collider other) => true;

    public void KinematicCollision(RayCastHit hit) { }

    public void KinematicUnstuckEvent(Collider collider, Vector3 penetrationDirection, float penetrationDistance) { }

    public void KinematicGroundingEvent(GroundState groundingState, RayCastHit? hit)
    {
        GroundingState = groundingState;
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
        Vector3 targetVelocity = inputDirection * GetTargetSpeed();
        Vector3 velocityDelta = targetVelocity - movementVelocity;

        float magnitudeDelta = Acceleration * Time.DeltaTime;
        if (velocityDelta.Length > magnitudeDelta)
        {
            velocityDelta = velocityDelta.Normalized * magnitudeDelta;
        }

        movementVelocity += velocityDelta;
    }

    private void ApplyGravity()
    {
        if (GroundingState != GroundState.Grounded)
        {
            gravityVelocity += Physics.Gravity / 100 * Time.DeltaTime;
        }
        else gravityVelocity = new(0, 0, 0);
    }

    private Vector3 GetInputDirection()
    {
        Vector3 inputDirection = new(movementRight.Value, 0, movementForward.Value);
        inputDirection = inputDirection.Normalized;
        inputDirection = Transform.TransformDirection(inputDirection);

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

        return targetSpeed;
    }
}
