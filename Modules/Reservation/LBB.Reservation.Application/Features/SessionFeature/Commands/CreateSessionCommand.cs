using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Domain;
using LBB.Reservation.Domain.Aggregates.Session;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public sealed class CreateSessionCommand : ICommand<Result<int>>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public required Enums.SessionType Type { get; set; }

    [MaxLength(Session.MaxTitleLength)]
    [Required]
    public required string Title { get; set; }

    [MaxLength(Session.MaxDescriptionLength)]
    public required string Description { get; set; }

    [Required]
    public required DateTime Start { get; set; }

    [Required]
    public required DateTime End { get; set; }

    [MaxLength(Core.ValueObjects.Location.MaxLength)]
    public string Location { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
}

public sealed class CreateSessionCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateSessionCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
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
                    capacity: command.Capacity
                );
        if (session.IsFailed)
            return session.ToResult<int>();

        unitOfWork.RegisterChange(session.Value);

        await unitOfWork.CommitAsync(cancellationToken);

        return session.Value.Id;
    }
}
