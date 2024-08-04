using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer.Components;

/// <summary>
/// Displays a sprite on the item that points towards the target component.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedPinpointerSystem))]
public sealed partial class PinpointerComponent : BasePinpointerComponent
{
    // TODO: Type serializer oh god
    /// <summary>
    ///     Component entity must have to be tracked
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string? Component;

    /// <summary>
    ///     Whether or not the target name should be updated when the target is updated.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool UpdateTargetName;

    /// <summary>
    ///     Whether or not the target can be reassigned.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool CanRetarget;

    [DataField, ViewVariables, AutoNetworkedField]
    public bool IsActive = false;

    [ViewVariables, AutoNetworkedField]
    public Angle ArrowAngle;

    [ViewVariables, AutoNetworkedField]
    public Distance PreviousDistance = Distance.Unknown;

    [ViewVariables, AutoNetworkedField]
    public Distance DistanceToTarget = Distance.Unknown;

    [ViewVariables]
    public bool HasTarget => DistanceToTarget != Distance.Unknown;
}

[Serializable, NetSerializable]
public enum Distance : byte
{
    Unknown,
    Reached,
    Close,
    Medium,
    Far
}
