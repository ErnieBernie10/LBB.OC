using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentResults;
using FluentValidation;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Domain;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using LBB.Reservation.Infrastructure.DataModels;
using Microsoft.Extensions.Localization;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public sealed class CreateSessionCommand : ICommand<Result<int>>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Enums.SessionType Type { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = "";
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public string Location { get; set; } = "";
    public int? Capacity { get; set; }

    public Session ToDataModel()
    {
        return new Session()
        {
            Type = (int)Type,
            Title = Title,
            Description = Description,
            Start = Start,
            End = End,
            Location = Location,
            Capacity = Capacity ?? 1,
        };
    }
}

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(DbConstraints.Session.MaxTitleLength);
        RuleFor(x => x.Description).MaximumLength(DbConstraints.Session.MaxDescriptionLength);
        RuleFor(x => x.Location).MaximumLength(DbConstraints.Session.MaxLocationLength);
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Start).GreaterThan(x => x.End);
        RuleFor(x => x.End).LessThan(x => x.Start);
        RuleFor(x => x.Type).IsInEnum();
    }
}

public sealed class CreateSessionCommandHandler(
    IValidator<CreateSessionCommand> validator,
    LbbDbContext context,
    IOutboxService outboxService
) : ICommandHandler<CreateSessionCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validation = validator.Validate(command);
        var result = validation.IsValid
            ? Result.Ok()
            : Result.Fail(new ValidationError(validation));

        if (result.IsFailed)
            return result;

        var id = await Persist(command, cancellationToken);
        return Result.Ok(id);
    }

    private async Task<int> Persist(
        CreateSessionCommand command,
        CancellationToken cancellationToken
    )
    {
        var session = command.ToDataModel();
        context.Sessions.Add(session);
        var tran = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await outboxService.PublishAsync(
            nameof(Session),
            session.Id.ToString(),
            new SessionCreated(session.Id)
        );
        await context.SaveChangesAsync(cancellationToken);

        await tran.CommitAsync(cancellationToken);

        return session.Id;
    }
}
