using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Pinpointer.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

public abstract class SharedPinpointerSystem : EntitySystem
{
    [Dependency] protected readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PinpointerComponent, GotEmaggedEvent>(OnEmagged);
        SubscribeLocalEvent<PinpointerComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<PinpointerComponent, ExaminedEvent>(OnExamined);

        SubscribeLocalEvent<PinpointerMultiTargetComponent, AfterInteractEvent>(OnMultipleAfterInteract);
    }

    /// <summary>
    ///     Set the target if capable
    /// </summary>
    private void OnAfterInteract(Entity<PinpointerComponent> pinpointer, ref AfterInteractEvent args)
    {
        var comp = pinpointer.Comp;

        if (!args.CanReach || args.Target is not { } target)
            return;

        if (!comp.CanRetarget || comp.IsActive)
            return;

        args.Handled = true;

        StartRetarget(args.User, target, comp.RetargetTime);
    }

    private void OnMultipleAfterInteract(Entity<PinpointerMultiTargetComponent> pinpointer, ref AfterInteractEvent args)
    {
        var comp = pinpointer.Comp;

        if (!args.CanReach || args.Target is not { } target)
            return;;

        if (!comp.CanAddTargets)
            return;

        args.Handled = true;

        if (comp.AdditionalTargetsQueue.Count >= comp.AdditionalTargetsCount)
        {
            comp.AdditionalTargetsQueue.Dequeue();
            comp.AdditionalTargetsQueue.Enqueue(target);
        }
        _adminLogger.Add(LogType.Action, LogImpact.Low, $"{ToPrettyString(args.User):player} added {ToPrettyString(target):target} as target to {ToPrettyString(pinpointer):pinpointer}");
    }

    private void StartRetarget(EntityUid user, EntityUid target, float time)
    {
        var doAfterEvent = new PinpointerTargetDoAfterEvent();
        var doAfterArgs = new DoAfterArgs(EntityManager, user, time, doAfterEvent, target)
        {
            BreakOnHandChange = true
        };

        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    /// <summary>
    ///     Set pinpointers target to track
    /// </summary>
    public virtual void SetTarget(EntityUid uid, EntityUid? target, PinpointerComponent? pinpointer = null)
    {
        if (!Resolve(uid, ref pinpointer))
            return;

        if (pinpointer.Target == target)
            return;

        pinpointer.Target = target;
        if (pinpointer.UpdateTargetName)
            pinpointer.TargetName = target == null ? null : Identity.Name(target.Value, EntityManager);
        if (pinpointer.IsActive)
            UpdateDirectionToTarget(uid, pinpointer);
    }

    /// <summary>
    ///     Update direction from pinpointer to selected target (if it was set)
    /// </summary>
    protected virtual void UpdateDirectionToTarget(EntityUid uid, PinpointerComponent? pinpointer = null)
    {

    }

    private void OnExamined(EntityUid uid, PinpointerComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange || component.TargetName == null)
            return;

        args.PushMarkup(Loc.GetString("examine-pinpointer-linked", ("target", component.TargetName)));
    }

    /// <summary>
    ///     Manually set distance from pinpointer to target
    /// </summary>
    public void SetDistance(EntityUid uid, Distance distance, PinpointerComponent? pinpointer = null)
    {
        if (!Resolve(uid, ref pinpointer))
            return;

        if (distance == pinpointer.DistanceToTarget)
            return;

        pinpointer.PreviousDistance = pinpointer.DistanceToTarget;
        pinpointer.DistanceToTarget = distance;
        Dirty(uid, pinpointer);
    }

    /// <summary>
    ///     Try to manually set pinpointer arrow direction.
    ///     If difference between current angle and new angle is smaller than
    ///     pinpointer precision, new value will be ignored and it will return false.
    /// </summary>
    public bool TrySetArrowAngle(EntityUid uid, Angle arrowAngle, PinpointerComponent? pinpointer = null)
    {
        if (!Resolve(uid, ref pinpointer))
            return false;

        if (pinpointer.ArrowAngle.EqualsApprox(arrowAngle, pinpointer.Precision))
            return false;

        pinpointer.ArrowAngle = arrowAngle;
        Dirty(uid, pinpointer);

        return true;
    }

    /// <summary>
    ///     Activate/deactivate pinpointer screen. If it has target it will start tracking it.
    /// </summary>
    public void SetActive(EntityUid uid, bool isActive, PinpointerComponent? pinpointer = null)
    {
        if (!Resolve(uid, ref pinpointer))
            return;
        if (isActive == pinpointer.IsActive)
            return;

        pinpointer.IsActive = isActive;
        Dirty(uid, pinpointer);
    }


    /// <summary>
    ///     Toggle Pinpointer screen. If it has target it will start tracking it.
    /// </summary>
    /// <returns>True if pinpointer was activated, false otherwise</returns>
    public virtual bool TogglePinpointer(EntityUid uid, PinpointerComponent? pinpointer = null)
    {
        if (!Resolve(uid, ref pinpointer))
            return false;

        var isActive = !pinpointer.IsActive;
        SetActive(uid, isActive, pinpointer);
        return isActive;
    }

    private void OnEmagged(EntityUid uid, PinpointerComponent component, ref GotEmaggedEvent args)
    {
        args.Handled = true;
        component.CanRetarget = true;
    }
}
