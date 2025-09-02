using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LBB.OC.Reservation;

public interface ISpaProvider
{
    bool Exists { get; }
    byte[]? Bytes { get; }
}

public sealed class SpaProvider : ISpaProvider
{
    private readonly IWebHostEnvironment _env;
    public bool Exists { get; }

    private byte[]? _bytes;
    public byte[]? Bytes
    {
        get => _env.IsDevelopment() ? GetIndex() : _bytes;
        private init => _bytes = value;
    }

    private byte[] GetIndex()
    {
        var ocRoot = _env.ContentRootPath;
        var moduleWebRoot = Path.GetFullPath(Path.Combine(ocRoot, $"../{Constants.ModuleName}/wwwroot"));
        var indexPath = Path.Combine(moduleWebRoot, "index.html");
        if (File.Exists(indexPath))
            return File.ReadAllBytes(indexPath);
        return [];
    }

    public SpaProvider(IWebHostEnvironment env)
    {
        _env = env;
        var index = GetIndex();
        Bytes = index;
        Exists = index.Length != 0;
    }
}
