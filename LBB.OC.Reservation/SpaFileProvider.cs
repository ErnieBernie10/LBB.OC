using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using OrchardCore.Modules;
using OrchardCore.Modules.FileProviders;

namespace LBB.OC.Reservation;
internal class SpaFileProvider : IFileProvider
{
    readonly Module _module;

    static string IndexFilePath => "index.html";

    public SpaFileProvider (IApplicationContext applicationContext)
    {
        var application = applicationContext.Application;
        _module = application.GetModule(GetType().Assembly.GetName().Name);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return string.IsNullOrEmpty(NormalizePath(subpath))
            ? new EmbeddedDirectoryContents(new[] { _module.GetFileInfo(IndexFilePath) })
            : NotFoundDirectoryContents.Singleton;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var fileSubPath = Path.Combine(Module.WebRoot, NormalizePath(subpath) ?? IndexFilePath);
        return _module.GetFileInfo(fileSubPath);
    }

    public IChangeToken Watch(string filter) { return NullChangeToken.Singleton; }

    static string NormalizePath(string path) => path?.Replace('\\', '/').Trim('/').Replace("//", "/");
}
