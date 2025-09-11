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
                ? Session.CreateIndividual(command)
                : Session.CreateGroup(command);
        if (session.IsFailed)
            return session.ToResult<int>();

        unitOfWork.RegisterChange(session.Value);

        await unitOfWork.CommitAsync(cancellationToken);

        return session.Value.Id;
    }
}
