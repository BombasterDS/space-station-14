namespace Content.Client.Infocomm;

public sealed class InfocommConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private InfocommConsole? _menu;

    public InfocommConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = new InfocommConsole();
        _menu.OnClose += Close;
        _menu.OpenCentered();
    }
}

