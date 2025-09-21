
using System.Security.Cryptography;
using System.Text;

namespace LinkService.Application;

public interface ISlugService
{
    string NewSlug(int len = 7);
}

public class SlugService
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public string NewSlug(int len = 7)
    {
        var bytes = RandomNumberGenerator.GetBytes(len);
        var sb = new StringBuilder(len);
        foreach (var b in bytes)
        {
            sb.Append(Alphabet[b % Alphabet.Length]);
        }
        return sb.ToString();
    }
}