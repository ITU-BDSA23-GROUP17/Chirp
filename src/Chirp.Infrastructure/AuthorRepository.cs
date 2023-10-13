using System.Data;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class AuthorRepository : IAuthorRepository, IDisposable
    {
        public AuthorDTO CreateNewAuthor(string Name, string Email)
        {
            Guid guid = Guid.NewGuid();
            return new AuthorDTO(guid.ToString(), Name, Email,new List<CheepDTO>());
        }

        public CheepDTO CreateNewCheepAsAuthor(string AuthorId, AuthorDTO Author, string Text)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public AuthorDTO GetAuthorByEmail(string Email)
        {
            throw new NotImplementedException();
        }

        public AuthorDTO GetAuthorById(string AuthorId)
        {
            throw new NotImplementedException();
        }

        public AuthorDTO GetAuthorByName(string Name)
        {
            throw new NotImplementedException();
        }
    }
}
