using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace LBB.OC.Reservation;

public interface ISpaProvider
{
    byte[]? GetBytes(string? lang);
}

public sealed class SpaProvider : ISpaProvider
{
    private readonly IWebHostEnvironment _env;

    public byte[]? GetBytes(string? lang = "en")
    {
        lang ??= CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        if (_bytes.TryGetValue(lang, out var bytes))
        {
            return bytes;
        }
        _bytes[lang] = GetIndex(lang);
        return _bytes[lang];
    }

    private Dictionary<string, byte[]> _bytes = new Dictionary<string, byte[]>();

    private byte[] GetIndex(string? lang)
    {
        if (lang == null)
            lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        var ocRoot = _env.ContentRootPath;
        var moduleWebRoot = Path.GetFullPath(Path.Combine(ocRoot, $"../Modules/Reservation/{Constants.ModuleName}/wwwroot/" + lang));
        var indexPath = Path.Combine(moduleWebRoot, "index.html");
        if (File.Exists(indexPath))
            return File.ReadAllBytes(indexPath);
        return [];
    }

    public SpaProvider(IWebHostEnvironment env)
    {
        _env = env;
    }
}
