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
        Task DeleteAuthorAsync(int authorId);
        Task UpdateAuthorAsync(AuthorDTO author);

        Task SendCheepAsync(string message, AuthorInfoDTO author);
        Task<AuthorDTO?> GetStatusByAuthorID(string authorId);
    }
}