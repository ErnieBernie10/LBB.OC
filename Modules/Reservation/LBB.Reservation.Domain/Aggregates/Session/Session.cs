using System.Runtime.CompilerServices;
using FluentResults;
using LBB.Core;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Domain.Aggregates.Session.Policies;
using LBB.Reservation.Domain.Contracts.Policy;

namespace LBB.Reservation.Domain.Aggregates.Session;

public sealed class Session : AggregateRoot
{
    private List<Reservation> _reservations;
    private readonly IReservationPolicy _reservationPolicy;
    public Timeslot Timeslot { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Location { get; private set; }
    public Enums.SessionType SessionType { get; private set; }
    public Capacity Capacity { get; private set; }

    internal Session(
        int id,
        Enums.SessionType sessionType,
        Timeslot timeslot,
        string title,
        string description,
        string location,
        Capacity capacity,
        List<Reservation> reservations
    )
        : this(sessionType, timeslot, title, description, location, capacity, reservations)
    {
        Id = id;
    }

    public Session(
        Enums.SessionType sessionType,
        Timeslot timeslot,
        string title,
        string description,
        string location,
        Capacity capacity,
        List<Reservation> reservations
    )
    {
        Timeslot = timeslot;
        Title = title;
        Description = description;
        Location = location;
        SessionType = sessionType;
        Capacity = capacity;
        _reservations = reservations;
        _reservationPolicy =
            SessionType == Enums.SessionType.Group
                ? new GroupSessionReservationPolicy()
                : new IndividualSessionReservationPolicy();
        AddDomainEvent(new SessionCreatedEvent(this));
    }

    public IReadOnlyList<Reservation> Reservations
    {
        get => _reservations;
        set => _reservations = value.ToList();
    }

    public Result UpdateInfo(
        string title,
        string description,
        string location,
        Timeslot timeslot,
        int newCapacity
    )
    {
        Location = location;
        Timeslot = timeslot;
        Title = title;
        Description = description;

        var result = Capacity.SetMax(newCapacity, nameof(newCapacity));

        if (result.IsFailed)
            return result.ToResult();

        AddDomainEvent(new SessionInfoUpdatedEvent(this));

        return Result.Ok();
    }

    public Result AddReservation(IAddReservationCommand command)
    {
        var canBook = _reservationPolicy.CanBook(this);
        if (canBook.IsFailed)
            return canBook;

        var reservation = Reservation.Create(command);
        if (reservation.IsFailed)
            return reservation.ToResult();

        var result = Capacity.Add(command.AttendeeCount ?? 1, nameof(command.AttendeeCount));
        if (result.IsFailed)
            return result.ToResult();

        Capacity = result.Value;
        _reservations.Add(reservation.Value);

        AddDomainEvent(new ReservationAddedEvent(this, reservation.Value));

        return Result.Ok();
    }

    public override IEnumerable<INotification> GatherDomainEvents()
    {
        foreach (var e in DomainEvents)
            yield return e;

        foreach (var r in Reservations)
        foreach (var e in r.DomainEvents)
            yield return e;
    }

    public Result Delete()
    {
        AddDomainEvent(new SessionDeletedEvent(this));
        return Result.Ok();
    }

    public Result CancelReservation(int commandReservationId)
    {
        var reservation = Reservations.FirstOrDefault(r => r.Id == commandReservationId);

        if (reservation == null)
            return Result.Fail(new NotFoundError("Reservation not found"));

        _reservations.Remove(reservation);

        AddDomainEvent(new ReservationRemovedEvent(this, reservation));

        return Result.Ok();
    }
}
