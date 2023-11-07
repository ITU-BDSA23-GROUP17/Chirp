using System.Data;
using Chirp.Core;
using Microsoft.VisualBasic;


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

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AuthorDTO? GetAuthorByEmail(string Email)
        {
            var Author = context.Authors.Where(a => a.Email == Email).FirstOrDefault();
            if (Author != null)
            {
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList());
            }
            else
            {
                return null;
            }
        }

        public AuthorDTO? GetAuthorById(string AuthorId)
        {
            var Author = context.Authors.Find(AuthorId);
            if (Author != null)
            {
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList());
            }
            else
            {
                return null;
            }
        }

        public AuthorDTO? GetAuthorByName(string Name)
        {
            var Author = context.Authors.Where(a => a.Name == Name).FirstOrDefault();
            if (Author != null)
            {
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList());
            }
            else
            {
                return null;
            }
        }

        void IAuthorRepository.DeleteAuthor(int authorId)
        {
            var author = context.Authors.Find(authorId);
            if (author != null)
            {
                context.Remove(author);
            }
        }

        void IAuthorRepository.UpdateAuthor(AuthorDTO author)
        {
            throw new NotImplementedException();
        }
    }
}
