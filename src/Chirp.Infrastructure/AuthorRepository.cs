using System.Data;
using Chirp.Core;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure
{
    public class AuthorRepository : IAuthorRepository
    {
        private ChirpDBContext context;

        public AuthorRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public async Task InsertAuthorAsync(string Name, string Email)
        {
            Guid guid = Guid.NewGuid();
            await context.Authors.AddAsync(new Author() { AuthorId = guid.ToString(), Name = Name, Email = Email, Status = "ONLINE" });
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<AuthorDTO?> GetAuthorByEmailAsync(string Email)
        {
            var Author = await context.Authors.Where(a => a.Email == Email).FirstOrDefaultAsync();
            if (Author != null)
            {
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Status, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList());
            }
            else
            {
                return null;
            }
        }

        public async Task<AuthorDTO?> GetAuthorByIdAsync(string AuthorId)
        {
            var Author = await context.Authors.FindAsync(AuthorId);
            if (Author != null)
            {
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Status, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList());
            }
            else
            {
                return null;
            }
        }

        public async Task<AuthorDTO?> GetAuthorByNameAsync(string Name)
        {
            var Author = await context.Authors.Where(a => a.Name == Name).FirstOrDefaultAsync();
            if (Author != null)
            {
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Status, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList());
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteAuthorAsync(int authorId)
        {
            var author = await context.Authors.FindAsync(authorId);
            if (author != null)
            {
                context.Remove(author);
            }
        }

        public async Task<string> GetStatusOfAuthorByID(int authorId)
        {
            var author = await context.Authors.FindAsync(authorId);
            if (author != null){
                return author.Status;
            } else {
                return null;
            }
        }

        public async Task SendCheepAsync(string message, AuthorInfoDTO authorInfoDTO)
        {
            var guid = Guid.NewGuid().ToString();
            var newCheepDTO = new CheepDTO(guid, message, DateTime.Now, authorInfoDTO.Name, authorInfoDTO.AuthorId);
            var author = await context.Authors.FindAsync(authorInfoDTO.AuthorId);

            if (author == null)
            {
                // Handle the case when the author is not found
                throw new Exception($"Author with id {authorInfoDTO.AuthorId} not found");
            }

            author.Cheeps.Add(new Cheep
            {
                CheepId = newCheepDTO.Id,
                Text = newCheepDTO.Message,
                TimeStamp = newCheepDTO.TimeStamp,
                Author = author,
                AuthorId = author.AuthorId
            });

            context.Authors.Update(author);
            await context.SaveChangesAsync();
        }

        public async Task<List<AuthorDTO>> GetAuthorsByIdsAsync(List<string> authorIDs)
        {
            var authors = await context.Authors
                 .Where(a => authorIDs
                 .Contains(a.AuthorId))
                 .ToListAsync();

            var authorDTOs = authors.Select(a => new AuthorDTO(a.AuthorId, a.Name, a.Email, a.Status, a.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId)).ToList())).ToList();
            return authorDTOs;
        }

        public async Task UpdateAuthorAsync(AuthorDTO author)
        {
            var authorToUpdate = await context.Authors.FindAsync(author.AuthorId);
            if (authorToUpdate != null)
            {
                authorToUpdate.Name = author.Name;
                authorToUpdate.Email = author.Email;
                context.Authors.Update(authorToUpdate);
            }
        }

    }
}