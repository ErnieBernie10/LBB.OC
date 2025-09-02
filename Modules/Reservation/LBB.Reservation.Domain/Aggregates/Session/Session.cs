using FluentResults;
using LBB.Core;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
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
        short sessionType,
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
            Timeslot.Create(start, end).Value,
            title,
            description,
            Location.Create(location).Value,
            Capacity.Create(capacity).Value,
            reservations
        ) { }

    public Session(
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

    public static Result<Session> CreateIndividual(
        string title,
        string description,
        string location,
        DateTime start,
        DateTime end
    )
    {
        var info = ValidateInfo(title, description);
        var t = Timeslot.Create(start, end);
        var l = Location.Create(location);
        var c = Capacity.Create(1);
        var result = Result.Merge(t, l, c, info);
        if (result.IsFailed)
            return result;

        var session = new Session(
            Enums.SessionType.Individual,
            t.Value,
            title,
            description,
            l.Value,
            c.Value,
            new List<Reservation>()
        );
        session.AddDomainEvent(new SessionCreatedEvent(session));
        return session;
    }

    private static ResultBase ValidateInfo(string title, string description)
    {
        if (string.IsNullOrEmpty(title))
            return Result.Fail("Title cannot be empty");
        if (title.Length > MaxTitleLength)
            return Result.Fail($"Title cannot be longer than {MaxTitleLength} characters");
        if (description.Length > MaxDescriptionLength)
            return Result.Fail(
                $"Description cannot be longer than {MaxDescriptionLength} characters"
            );

        return Result.Ok();
    }

    public static Result<Session> CreateGroup(
        string title,
        string description,
        string location,
        DateTime start,
        DateTime end,
        int capacity
    )
    {
        var info = ValidateInfo(title, description);
        var t = Timeslot.Create(start, end);
        var l = Location.Create(location);
        var c = Capacity.Create(capacity);
        var result = Result.Merge(t, l, c, info);
        if (result.IsFailed)
            return result;

        var session = new Session(
            Enums.SessionType.Individual,
            t.Value,
            title,
            description,
            l.Value,
            c.Value,
            new List<Reservation>()
        );
        session.AddDomainEvent(new SessionCreatedEvent(session));
        return session;
    }

    public Result UpdateInfo(string title, string description, string location, int requestCapacity)
    {
        Title = title;
        Description = description;
        var c = Capacity.Create(requestCapacity, Reservations.Count);
        var l = Location.Create(location);
        var result = Result.Merge(c, l);
        if (result.IsFailed)
            return result;

        Capacity = c.ValueOrDefault;
        Location = l.ValueOrDefault;

        AddDomainEvent(new SessionInfoUpdatedEvent(this));

        return Result.Ok();
    }

    public Result AddReservation(
        string firstname,
        string lastname,
        string email,
        string phone,
        int memberCount
    )
    {
        var canBook = _reservationPolicy.CanBook(this);
        if (canBook.IsFailed)
            return canBook;

        var reservation = Reservation.Create(firstname, lastname, email, phone, memberCount);
        if (reservation.IsFailed)
            return reservation.ToResult();

        var result = Capacity.Add(memberCount);
        if (result.IsFailed)
            return result.ToResult();

        Capacity = result.Value;
        _reservations.Add(reservation.Value);

        AddDomainEvent(new ReservationAddedEvent(this, reservation.Value));

        return Result.Ok();
    }

    public Result UpdateTimeslot(DateTime requestStart, DateTime requestEnd)
    {
        var ts = Timeslot.Create(requestStart, requestEnd);
        if (ts.IsFailed)
            return ts.ToResult();

        Timeslot = ts.ValueOrDefault;
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
