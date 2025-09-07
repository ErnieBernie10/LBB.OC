using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace LBB.OC.Reservation;

public static class ApplicationBuilderExtensions
{

    private static string GetModuleWebRoot(IWebHostEnvironment? env)
    {
        if (env == null)
            return string.Empty;

        var ocRoot = env.ContentRootPath;
        return Path.Combine(ocRoot, $"../Modules/Reservation/{Constants.ModuleName}/wwwroot");
    }
    public static void UseReservationMiddleware(this IApplicationBuilder builder, IServiceProvider serviceProvider)
    {
        var env = serviceProvider.GetService<IWebHostEnvironment>();

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(GetModuleWebRoot(env)),
        });

    }
}