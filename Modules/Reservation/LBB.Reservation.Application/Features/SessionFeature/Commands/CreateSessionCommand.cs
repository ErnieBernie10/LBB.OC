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
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Infrastructure;
using Microsoft.Extensions.Localization;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public sealed class CreateSessionCommand : ICreateGroupSessionCommand, ICommand<Result<int>>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Enums.SessionType Type { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = "";
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public string Location { get; set; } = "";
    public int? Capacity { get; set; }
}

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(DbConstraints.Session.MaxTitleLength);
        RuleFor(x => x.Description).MaximumLength(DbConstraints.Session.MaxDescriptionLength);
        RuleFor(x => x.Location).MaximumLength(DbConstraints.Session.MaxLocationLength);
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Start).GreaterThan(DateTime.MinValue).LessThan(DateTime.MaxValue);
        RuleFor(x => x.End).GreaterThan(DateTime.MinValue).LessThan(DateTime.MaxValue);
        RuleFor(x => x.Type).IsInEnum();
    }
}

public sealed class CreateSessionCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<CreateSessionCommand> validator
) : ICommandHandler<CreateSessionCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validation = validator.Validate(command);
        var validationResult = validation.IsValid
            ? Result.Ok()
            : Result.Fail(new ValidationError(validation));

        var t = Timeslot.Create(
            command.Start,
            command.End,
            nameof(command.Start),
            nameof(command.End)
        );
        var c = command.Type switch
        {
            Enums.SessionType.Individual => Capacity.Create(1, 1.ToString()),
            Enums.SessionType.Group => Capacity.Create(
                command.Capacity ?? 12,
                nameof(command.Capacity)
            ),
            _ => Capacity.Create(),
        };
        var result = Result.Merge(c, t, validationResult);
        if (result.IsFailed)
            return result;

        var session = new Session(
            command.Type,
            t.Value,
            command.Title,
            command.Description,
            command.Location,
            c.Value,
            []
        );

        unitOfWork.RegisterChange(session);

        await unitOfWork.CommitAsync(cancellationToken);

        return session.Id;
    }
}
