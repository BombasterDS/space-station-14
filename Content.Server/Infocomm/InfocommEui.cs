using Content.Server.EUI;

namespace Content.Server.Infocomm;

public sealed class InfocommEui : BaseEui
{
    private readonly EntityUid _station;

    public readonly EntityUid? Owner;

    public InfocommEui(EntityUid station, EntityUid? owner)
    {
        _station = station;
        Owner = owner;
    }
}

