using Robust.Shared.GameStates;

namespace Content.Shared.Pinpointer.Components;

/// <summary>
/// Version of pinpointer with UI where you can select on from multiple targets
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedPinpointerSystem))]
public sealed partial class PinpointerMultiTargetComponent : Component
{
    /// <summary>
    ///     Tracking entities with specific components
    /// </summary>
    [DataField]
    public List<string>? TrackingComponents;

    /// <summary>
    ///     Queue of targets user can link pinpointer to
    /// </summary>
    public Queue<EntityUid> AdditionalTargetsQueue = new Queue<EntityUid>();

    /// <summary>
    ///     Amount of targets that can be assigned to pinpointer
    /// </summary>
    public byte AdditionalTargetsCount = 1;

    public List<EntityUid> EntitiesTrackList = new List<EntityUid>();

    /// <summary>
    /// Can add more targets by clicking on it?
    /// </summary>
    [DataField]
    public bool CanAddTargets = false;

    [DataField, ViewVariables, AutoNetworkedField]
    public bool IsActive = false;

    [ViewVariables, AutoNetworkedField]
    public Angle ArrowAngle;

    [ViewVariables, AutoNetworkedField]
    public Distance DistanceToTarget = Distance.Unknown;

    [ViewVariables]
    public bool HasTarget => DistanceToTarget != Distance.Unknown;
}
