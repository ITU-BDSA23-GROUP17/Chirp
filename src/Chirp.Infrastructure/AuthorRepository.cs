using System.Data;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class AuthorRepository : IAuthorRepository, IDisposable
    {

        private ChirpDBContext context;

        public AuthorRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public void InsertAuthor(string Name, string Email)
        {
            Guid guid = Guid.NewGuid();
            context.Authors.Add(new Author() { AuthorId = guid.ToString(), Name = Name, Email = Email });
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
            var Author = context.Authors.Where(a => a.Email == Email).FirstOrDefault();
            return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name)).ToList());
        }

        public AuthorDTO GetAuthorById(string AuthorId)
        {
            var Author = context.Authors.Find(AuthorId);
            return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name)).ToList());
        }

        public AuthorDTO GetAuthorByName(string Name)
        {
            var Author = context.Authors.Where(a => a.Name == Name).FirstOrDefault();
            return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name)).ToList());
        }

        void IAuthorRepository.DeleteAuthor(int authorId)
        {
            context.Remove(context.Authors.Find(authorId));
        }

        void IAuthorRepository.UpdateAuthor(AuthorDTO author)
        {
            throw new NotImplementedException();
        }
    }
}
