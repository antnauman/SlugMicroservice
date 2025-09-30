
using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using SlugMicroservice.Models;
using SlugService.Contract.v01;

namespace SlugMicroservice.Application;

public interface ISlugService
{
    string NewSlug(int len = 7);
}

public class SlugGrpcService : SlugService.Contract.v01.SlugService.SlugServiceBase
{
    private readonly ILinkRepository linkRepository;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public SlugGrpcService(ILinkRepository linkRepository)
    {
        this.linkRepository = linkRepository;
    }

    public override async Task<GenerateSlugResponse> GenerateSlug(GenerateSlugRequest request, ServerCallContext context)
    {
        var slug = NewSlug();

        var slugResponse = new GenerateSlugResponse() { Code = slug };
        var isCreated = await linkRepository.CreateLink(request.Url, slug);
        if (!isCreated)
        { throw new Exception("Slug not created"); }

        return slugResponse;
    }

    public override async Task<ResolveSlugResponse> ResolveSlug(ResolveSlugRequest request, ServerCallContext context)
    {
        var link = await linkRepository.GetLinkByCode(request.Code);
        if (link is null)
        { throw new Exception("Slug not found"); }

        return new ResolveSlugResponse() { Id = link.Id.ToString(), Url = link.TargetUrl };
    }

    private string NewSlug(int len = 7)
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