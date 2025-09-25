using FluentResults;
using FluentValidation;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using LBB.Reservation.Infrastructure.DataModels;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateSessionInfoCommand : ICommand<Result>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public int SessionId { get; set; }
}

public class SessionInfoValidator : AbstractValidator<UpdateSessionInfoCommand>
{
    public SessionInfoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(DbConstraints.Session.MaxTitleLength);
        RuleFor(x => x.Description).MaximumLength(DbConstraints.Session.MaxDescriptionLength);
        RuleFor(x => x.Location).MaximumLength(DbConstraints.Session.MaxLocationLength);
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Start).LessThan(x => x.End);
        RuleFor(x => x.End).GreaterThan(x => x.Start);
    }
}

public class UpdateSessionInfoCommandHandler(
    IValidator<UpdateSessionInfoCommand> validator,
    LbbDbContext context,
    IOutboxService outboxService
) : ICommandHandler<UpdateSessionInfoCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateSessionInfoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validation = validator.Validate(command);
        var result = validation.IsValid
            ? Result.Ok()
            : Result.Fail(new ValidationError(validation));
        if (result.IsFailed)
            return result;

        var session = await context.Sessions.FindAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        session.Title = command.Title;
        session.Description = command.Description;
        session.Location = command.Location;
        session.Capacity = command.Capacity;
        session.Start = command.Start;
        session.End = command.End;
        session.UpdatedOn = DateTime.UtcNow;

        await outboxService.PublishAsync(
            nameof(Session),
            session.Id.ToString(),
            new SessionUpdated(session.Id),
            cancellationToken
        );
        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
