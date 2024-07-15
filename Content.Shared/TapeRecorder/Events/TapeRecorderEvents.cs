using Content.Shared.DoAfter;
using Content.Shared.TapeRecorder.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.TapeRecorder.Events;

[Serializable, NetSerializable]
public sealed partial class TapeCassetteRepairDoAfterEvent : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed class ToggleTapeRecorderMessage : BoundUserInterfaceMessage
{
    public bool Active;

    public ToggleTapeRecorderMessage(bool active)
    {
        Active = active;
    }
}

[Serializable, NetSerializable]
public sealed class ChangeModeTapeRecorderMessage : BoundUserInterfaceMessage
{
    public TapeRecorderMode Mode;

    public ChangeModeTapeRecorderMessage(TapeRecorderMode mode)
    {
        Mode = mode;
    }
}

[Serializable, NetSerializable]
public sealed class TapeRecorderState : BoundUserInterfaceState
{
    public bool Active;
    public TapeRecorderMode Mode;
    public bool HasCasette;
    public bool HasData;

    public TapeRecorderState(
        bool active,
        TapeRecorderMode mode,
        bool hasCasette,
        bool hasData)
    {
        Active = active;
        Mode = mode;
        HasCasette = hasCasette;
        HasData = hasData;
    }
}
