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
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 2000;

    public List<Reservation> _reservations;
    public IReservationPolicy _reservationPolicy;
    public Capacity Capacity { get; private set; }

    internal Session(
        int id,
        int sessionType,
        DateTime start,
        DateTime end,
        string title,
        string description,
        string location,
        int capacity,
        List<Reservation> reservations
    )
        : this(
            (Enums.SessionType)sessionType,
            Timeslot.Create(start, end, nameof(start), nameof(end)).Value,
            title,
            description,
            Location.Create(location, nameof(location)).Value,
            Capacity.Create(capacity, nameof(capacity)).Value,
            reservations
        )
    {
        Id = id;
    }

    private Session(
        Enums.SessionType sessionType,
        Timeslot timeslot,
        string title,
        string description,
        Location location,
        Capacity capacity,
        IReadOnlyList<Reservation> reservations
    )
    {
        Timeslot = timeslot;
        Title = title;
        Description = description;
        Location = location;
        Reservations = reservations;
        SessionType = sessionType;
        Capacity = capacity;
        _reservationPolicy =
            SessionType == Enums.SessionType.Group
                ? new GroupSessionReservationPolicy()
                : new IndividualSessionReservationPolicy();
    }

    public Timeslot Timeslot { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Location Location { get; private set; }
    public Enums.SessionType SessionType { get; private set; }

    public IReadOnlyList<Reservation> Reservations
    {
        get => _reservations;
        set => _reservations = value.ToList();
    }

    public static Result<Session> CreateIndividual(ICreateIndividualSessionCommand command)
    {
        var info = ValidateInfo(
            command.Title,
            command.Description,
            nameof(command.Title),
            nameof(command.Description)
        );
        var t = Timeslot.Create(
            command.Start,
            command.End,
            nameof(command.Start),
            nameof(command.End)
        );
        var l = Location.Create(command.Location, nameof(command.Location));
        var c = Capacity.Create(1, 1.ToString());
        var result = Result.Merge(t, l, c, info);
        if (result.IsFailed)
            return result;

        var session = new Session(
            Enums.SessionType.Individual,
            t.Value,
            command.Title,
            command.Description,
            l.Value,
            c.Value,
            new List<Reservation>()
        );
        session.AddDomainEvent(new SessionCreatedEvent(session));
        return session;
    }

    private static ResultBase ValidateInfo(
        string title,
        string description,
        string titlePropertyName,
        string descriptionPropertyName
    )
    {
        var errors = new List<IError>();
        if (string.IsNullOrEmpty(title))
            errors.Add(new NotEmptyError(titlePropertyName));
        if (title.Length > MaxTitleLength)
            errors.Add(new LengthExceededError(titlePropertyName, MaxTitleLength));
        if (description.Length > MaxDescriptionLength)
            errors.Add(new LengthExceededError(descriptionPropertyName, MaxDescriptionLength));

        if (errors.Count > 0)
            return Result.Fail(errors);

        return Result.Ok();
    }

    public static Result<Session> CreateGroup(ICreateGroupSessionCommand command)
    {
        var info = ValidateInfo(
            command.Title,
            command.Description,
            nameof(command.Title),
            nameof(command.Description)
        );
        var t = Timeslot.Create(
            command.Start,
            command.End,
            nameof(command.Start),
            nameof(command.End)
        );
        var l = Location.Create(command.Location, nameof(command.Location));
        var c = Capacity.Create(command.Capacity, nameof(command.Capacity));
        var result = Result.Merge(t, l, c, info);
        if (result.IsFailed)
            return result;

        var session = new Session(
            Enums.SessionType.Group,
            t.Value,
            command.Title,
            command.Description,
            l.Value,
            c.Value,
            new List<Reservation>()
        );
        session.AddDomainEvent(new SessionCreatedEvent(session));
        return session;
    }

    public Result UpdateInfo(IUpdateSessionInfoCommand command)
    {
        var infoResult = ValidateInfo(
            command.Title,
            command.Description,
            nameof(command.Title),
            nameof(command.Description)
        );
        var c = Capacity.Create(
            command.Capacity,
            Reservations.Count,
            nameof(command.Capacity),
            nameof(Reservations)
        );
        var l = Location.Create(command.Location, nameof(command.Location));
        var ts = Timeslot.Create(
            command.Start,
            command.End,
            nameof(command.Start),
            nameof(command.End)
        );

        var result = Result.Merge(c, l, infoResult, ts);
        if (result.IsFailed)
            return result;

        Timeslot = ts.ValueOrDefault;
        Capacity = c.ValueOrDefault;
        Location = l.ValueOrDefault;
        Title = command.Title;
        Description = command.Description;

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
}
