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
                Task UpdateAuthorStatusAsync(string email);
                Task DeleteAuthorAsync(string authorId);
                Task DeleteAuthorByNameAsync(string authorName);

                Task UpdateAuthorAsync(AuthorDTO author);

                Task<CheepDTO> SendCheepAsync(string message, AuthorInfoDTO author);
                Task InsertAuthorAsync(string? username, string email, string online);
        }
}