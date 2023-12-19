using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Chirp.Core
{
    public interface IAuthorRepository
    {
        Task<AuthorDTO?> GetAuthorByNameAsync(string name);

        Task<AuthorDTO?> GetAuthorByEmailAsync(string email);

        Task<AuthorDTO?> GetAuthorByIdAsync(string authorId);
        Task<List<AuthorDTO>> GetAuthorsByIdsAsync(List<string> authorIDs);
        Task SaveAsync();

        Task InsertAuthorAsync(string name, string email);
        Task<string?> GetAuthorStatusAsync(string Email);
        Task UpdateAuthorStatusAsync(string email);
        Task UpdateAuthorStatusOnline(string Email);
        Task UpdateAuthorStatusOffline(string Email);
        Task UpdateAuthorStatusUnavailable(string Email);
        Task DeleteAuthorAsync(string name);
        Task UpdateAuthorAsync(AuthorDTO author);

        Task<CheepDTO> SendCheepAsync(string message, AuthorInfoDTO author);
        Task InsertAuthorAsync(string? username, string email, string online);
    }
}