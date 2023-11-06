using Content.Server.Doors.Systems;
using Content.Server.Resist;
using Content.Shared.Charges.Systems;
using Content.Shared.Charges.Components;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Doors.Components;
using Content.Shared.Emag.Components;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Server.Unemmager;

public sealed class UnemmagerSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedChargesSystem _charges = default!;
    [Dependency] private readonly DoorBoltSystem _bolts = default!;
    [Dependency] private readonly DoorSystem _door = default!;
    [Dependency] private readonly SharedAccessSystem _access = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<UnemmagerComponent, AfterInteractEvent>(OnInteract);
    }

    public void OnInteract(EntityUid uid, UnemmagerComponent component, AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target is not { } target)
            return;

        bool unemagged = TryUnemag(uid, target, args.User);
        if (unemagged)
        {
            _audio.PlayPvs(component.Sound, uid, AudioParams.Default.WithVolume(8));
            _popup.PopupEntity(Loc.GetString("Исправление замыканий микросхемы..."), args.User, args.User, PopupType.Medium);
        }

    }

    public bool TryUnemag(EntityUid uid, EntityUid target, EntityUid user)
    {
        EntityManager.TryGetComponent<LimitedChargesComponent>(uid, out var charges);
        if (_charges.IsEmpty(uid, charges))
        {
            _popup.PopupEntity(Loc.GetString("Недостаточно заряда..."), user, user, PopupType.Medium);
            return false;
        }

        if (EntityManager.HasComponent<EmaggedComponent>(target))
        {
            EntityManager.RemoveComponent<EmaggedComponent>(target);
            Unbolt(target);
            ReturnLock(target);
            EnableAccess(target);
            RemoveCharge(uid, charges);
            return true;
        }
        return false;
    }

    public void ReturnLock(EntityUid target)
    {
        if (EntityManager.HasComponent<ResistLockerComponent>(target) && !EntityManager.HasComponent<LockComponent>(target))
            EntityManager.AddComponent<LockComponent>(target);
    }

    public void EnableAccess(EntityUid target)
    {
        if (EntityManager.TryGetComponent<AccessReaderComponent>(target, out var accessReader))
            accessReader.Enabled = true;
    }

    public void RemoveCharge(EntityUid uid, LimitedChargesComponent? charges)
    {
        if (charges != null)
            _charges.UseCharge(uid, charges);        
    }

    public void Unbolt(EntityUid target)
    {
        if (EntityManager.TryGetComponent<DoorBoltComponent>(target, out var bolt))
        {
            _bolts.SetBoltsWithAudio(target, bolt, false);
        }

        if (EntityManager.TryGetComponent<DoorComponent>(target, out var door))
        {
            _door.StartClosing(target);
        }
    }
}
