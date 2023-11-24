
namespace Chirp.Core;
public record AuthorDTO(string AuthorId, string Name, string Email, List<CheepDTO> Cheeps)
{
    public IEnumerable<char>? Status;
}
