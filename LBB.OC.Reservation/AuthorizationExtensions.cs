using Microsoft.Extensions.DependencyInjection;
using OrchardCore;

namespace LBB.OC.Reservation;

public static class AuthorizationExtensions
{
    public static void ConfigureReservationModuleAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.Policies.ManageReservations, policy =>
                policy.RequireRole(
                    OrchardCoreConstants.Roles.Administrator,
                    Constants.Roles.ReservationManager));
        });

    }
}