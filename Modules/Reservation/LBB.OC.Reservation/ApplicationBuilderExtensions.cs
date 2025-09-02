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
        builder.Use(async (context, next) =>
        {
            if (HttpMethods.IsGet(context.Request.Method))
            {
                var path = context.Request.Path.Value ?? string.Empty;

                // If requesting the tenant root ("/"), redirect to "/reservation/"
                if (string.Equals(path, "/", StringComparison.Ordinal) || string.IsNullOrEmpty(path))
                {
                    var qs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                    context.Response.Redirect($"{context.Request.PathBase}/{Constants.ModuleBasePath}/{qs}", permanent: false);
                    return;
                }

                // Normalize "/reservation" -> "/reservation/"
                if (string.Equals(path, $"/{Constants.ModuleBasePath}", StringComparison.OrdinalIgnoreCase))
                {
                    var qs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                    context.Response.Redirect($"{context.Request.PathBase}/{Constants.ModuleBasePath}/{qs}", permanent: false);
                    return;
                }
            }

            await next();
        });


        var env = serviceProvider.GetService<IWebHostEnvironment>();

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(GetModuleWebRoot(env))
        });

    }
}