using Robust.Shared.GameStates;

namespace Content.Shared.NavMap;

/// <summary>
/// This is used for objects which appear as doors on the navmap.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedNavMapSystem))]
public sealed partial class NavMapDoorComponent : Component
{

}
