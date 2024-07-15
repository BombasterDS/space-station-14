using Content.Shared.TapeRecorder.Components;
using Content.Shared.TapeRecorder.Events;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Client.TapeRecorder.Ui;

[UsedImplicitly]
public sealed class TapeRecorderBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private TapeRecorderMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = new(this);
        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    public void ToggleSwitch(bool active)
    {
        SendMessage(new ToggleTapeRecorderMessage(active));
    }

    public void ChangeMode(TapeRecorderMode mode)
    {
        SendMessage(new ChangeModeTapeRecorderMessage(mode));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        var castState = (TapeRecorderState) state;
        _menu?.UpdateState(castState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _menu?.Dispose();
    }
}

