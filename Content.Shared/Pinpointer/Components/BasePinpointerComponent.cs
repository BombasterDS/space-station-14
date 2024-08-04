namespace Content.Shared.Pinpointer.Components;

public abstract partial class BasePinpointerComponent : Component
{
    /// <summary>
    /// Distances states for sprite change
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float MediumDistance = 16f;

    [ViewVariables(VVAccess.ReadWrite)]
    public float CloseDistance = 8f;

    [ViewVariables(VVAccess.ReadWrite)]
    public float ReachedDistance = 1f;

    [ViewVariables(VVAccess.ReadWrite)]
    public float RetargetTime = 2f;

    /// <summary>
    ///     Pinpointer arrow precision in radians.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public double Precision = 0.09;

    /// <summary>
    ///     Name to display of the target being tracked.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string? TargetName;

    [ViewVariables]
    public EntityUid? Target = null;
}
