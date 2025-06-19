using FlaxEngine;

namespace FreakyController;

/// <summary>
/// CenterAndRange class.
/// </summary>
public struct CenterAndRange
{
    public Actor Target;
    public float MaxAngle;

    private readonly Camera camera;
    private readonly LookAround lookAround;

    public CenterAndRange(Camera camera, LookAround lookAround)
    {
        this.camera = camera;
        this.lookAround = lookAround;
    }

    public void Update()
    {
        if (Target == null) return;

        Vector3 toTarget = Target.Position - camera.Position;
        Vector3 forward = camera.Transform.Forward;

        var angle = Vector3.Angle(toTarget, forward);

        if (angle > MaxAngle)
        {
            Vector3 cross = Vector3.Cross(toTarget, forward);
            var rot = Quaternion.RotationAxis(cross, MaxAngle * Mathf.DegreesToRadians);

            Vector3 targetForward = toTarget * rot;
            lookAround.SetForward(targetForward, out _);
        }
    }
}
