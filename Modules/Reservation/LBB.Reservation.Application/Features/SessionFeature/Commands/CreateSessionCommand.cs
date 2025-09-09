using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentResults;
using FluentValidation;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain;
using LBB.Reservation.Domain.Aggregates.Session;
using Microsoft.Extensions.Localization;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public sealed class CreateSessionCommand : ICommand<Result<int>>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Enums.SessionType Type { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public string Location { get; set; } = "";
    public int? Capacity { get; set; }
}

public sealed class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithErrorCode(Constants.ErrorCodes.Validation.Required);
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
        RuleFor(x => x.Capacity).NotEmpty().When(c => c.Type == Enums.SessionType.Group).GreaterThan(0);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Location).MaximumLength(Core.ValueObjects.Location.MaxLength);
        RuleFor(x => x.Description).MaximumLength(Session.MaxDescriptionLength);
        RuleFor(x => x.Title).MaximumLength(Session.MaxTitleLength);
    }
}

public sealed class CreateSessionCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<CreateSessionCommandHandler> localizer, FluentValidation.IValidator<CreateSessionCommand> validator)
    : ICommandHandler<CreateSessionCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = validator.Validate(command);
        if (!result.IsValid)
            return Result.Fail(new ValidationError(result));
        // TODO: Execute the validator and map to result from FluentResults so it can be used to add to the modelstate
        // in the controller. The modelstate will only contain error codes. The UI will render based on the error codes.
        // Remove intl from backend entirely.
        var session =
            command.Type == Enums.SessionType.Individual
                ? Session.CreateIndividual(
                    title: command.Title,
                    description: command.Description,
                    location: command.Location,
                    start: command.Start,
                    end: command.End
                )
                : Session.CreateGroup(
                    title: command.Title,
                    description: command.Description,
                    location: command.Location,
                    start: command.Start,
                    end: command.End,
                    capacity: command.Capacity!.Value
                );
        if (session.IsFailed)
            return session.ToResult<int>();

        unitOfWork.RegisterChange(session.Value);

        await unitOfWork.CommitAsync(cancellationToken);

        return session.Value.Id;
    }
}
