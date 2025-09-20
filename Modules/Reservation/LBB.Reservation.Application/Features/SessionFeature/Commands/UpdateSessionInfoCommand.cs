using FluentResults;
using FluentValidation;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Infrastructure;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateSessionInfoCommand : IUpdateSessionInfoCommand, ICommand<Result>
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
        RuleFor(x => x.Start).GreaterThan(DateTime.MinValue).LessThan(DateTime.MaxValue);
        RuleFor(x => x.End).GreaterThan(DateTime.MinValue).LessThan(DateTime.MaxValue);
    }
}

public class UpdateSessionInfoCommandHandler(
    IUnitOfWork uow,
    IAggregateStore<Session, int> store,
    IValidator<UpdateSessionInfoCommand> validator
) : ICommandHandler<UpdateSessionInfoCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateSessionInfoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validation = validator.Validate(command);
        var validationResult = validation.IsValid
            ? Result.Ok()
            : Result.Fail(new ValidationError(validation));

        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var timeslot = Timeslot.Create(
            command.Start,
            command.End,
            nameof(command.Start),
            nameof(command.End)
        );

        var result = Result.Merge(timeslot, validationResult);
        if (result.IsFailed)
            return result.ToResult();

        var updateResult = session.UpdateInfo(
            command.Title,
            command.Description,
            command.Location,
            timeslot.Value,
            command.Capacity
        );
        if (updateResult.IsFailed)
            return updateResult;

        uow.RegisterChange(session);

        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
