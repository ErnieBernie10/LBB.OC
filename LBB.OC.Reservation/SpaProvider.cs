using Microsoft.AspNetCore.Hosting;

namespace LBB.OC.Reservation;

public interface ISpaProvider
{
    bool Exists { get; }
    byte[]? Bytes { get; }
}

public sealed class SpaProvider : ISpaProvider
{
    public bool Exists { get; }
    public byte[]? Bytes { get; }

    public SpaProvider(IWebHostEnvironment env)
    {
        var ocRoot = env.ContentRootPath;
        var moduleWebRoot = Path.GetFullPath(Path.Combine(ocRoot, "../LBB.OC.Reservation/wwwroot"));
        var indexPath = Path.Combine(moduleWebRoot, "index.html");

        if (File.Exists(indexPath))
        {
            Bytes = File.ReadAllBytes(indexPath);
            Exists = true;
        }
    }
}
