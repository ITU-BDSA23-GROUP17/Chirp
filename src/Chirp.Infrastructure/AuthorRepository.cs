using System.Data;
using Chirp.Core;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

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
            GithubClaims githubclaims = new GithubClaims();
            await context.Authors.AddAsync(new Author() { AuthorId = guid.ToString(), Name = Name, Email = Email, Status = "OFFLINE", Image = await githubclaims.GetGitHubClaimsUserImageAsync(Name) });
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
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Status, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image)).ToList(), Author.Image);

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
                return new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Status, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image)).ToList(), Author.Image);
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
                AuthorDTO newAuthorDTO = new AuthorDTO(Author.AuthorId, Author.Name, Author.Email, Author.Status, Author.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image)).ToList(), Author.Image);
                return newAuthorDTO;
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteAuthorAsync(string authorName)
        {
            Console.WriteLine(" deleting author" + authorName);

            var author = await context.Authors.Where(a => a.Name == authorName).FirstOrDefaultAsync();
            Console.WriteLine("author found" + author?.AuthorId);
            if (author != null)
            {
                context.Authors.Remove(author);
                await context.SaveChangesAsync();
                Console.WriteLine("Author deleted");


            }
            else
            {
                Console.WriteLine("Author not found");
            }

        }

        public async Task<string?> GetStatusOfAuthorByID(int authorId)
        {
            var author = await context.Authors.FindAsync(authorId);
            if (author != null)
            {
                return author.Status;
            }
            else
            {
                return null;
            }
        }

        public async Task<CheepDTO> SendCheepAsync(string message, AuthorInfoDTO authorInfoDTO)
        {
            var guid = Guid.NewGuid().ToString();
            var newCheepDTO = new CheepDTO(guid, message, DateTime.Now, authorInfoDTO.Name, authorInfoDTO.AuthorId, "");
            var author = await context.Authors.FindAsync(authorInfoDTO.AuthorId);

            if (author == null)
            {
                // Handle the case when the author is not found
                throw new Exception($"Author with id {authorInfoDTO.AuthorId} not found");
            }
            var cheep = new Cheep
            {
                CheepId = newCheepDTO.Id,
                Text = newCheepDTO.Message,
                TimeStamp = newCheepDTO.TimeStamp,
                Author = author,
                AuthorId = author.AuthorId
            };
            author.Cheeps.Add(cheep);

            context.Authors.Update(author);
            await context.SaveChangesAsync();
            //we return newCheepDTO in order to use it for adding the cheep to the hashtag repository if there are any hashtags...
            return newCheepDTO;
        }

        public async Task<List<AuthorDTO>> GetAuthorsByIdsAsync(List<string> authorIDs)
        {
            var authors = await context.Authors
                 .Where(a => authorIDs
                 .Contains(a.AuthorId))
                 .ToListAsync();

            var authorDTOs = authors.Select(a => new AuthorDTO(a.AuthorId, a.Name, a.Email, a.Status, a.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image)).ToList(), a.Name)).ToList();
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

        public async Task UpdateAuthorStatusAsync(string Email)
        {
            var authorToUpdate = await context.Authors.Where(a => a.Email == Email).FirstOrDefaultAsync();
            if (authorToUpdate != null)
            {
                if (authorToUpdate.Status.Equals("OFFLINE"))
                {
                    authorToUpdate.Status = "ONLINE";
                    context.Authors.Update(authorToUpdate);
                    await context.SaveChangesAsync();
                }
                else
                {
                    authorToUpdate.Status = "OFFLINE";
                    context.Authors.Update(authorToUpdate);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateAuthorStatusUnavailable(string Email)
        {
            var authorToUpdate = await context.Authors.Where(a => a.Email == Email).FirstOrDefaultAsync();
            if (authorToUpdate != null)
            {
                if (!authorToUpdate.Status.Equals("UNAVAILABLE"))
                {
                    authorToUpdate.Status = "UNAVAILABLE";
                    context.Authors.Update(authorToUpdate);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateAuthorStatusOnline(string Email)
        {
            var authorToUpdate = await context.Authors.Where(a => a.Email == Email).FirstOrDefaultAsync();
            if (authorToUpdate != null)
            {
                if (!authorToUpdate.Status.Equals("ONLINE"))
                {
                    authorToUpdate.Status = "ONLINE";
                    context.Authors.Update(authorToUpdate);
                    await context.SaveChangesAsync();
                }

            }
        }
        public async Task UpdateAuthorStatusOffline(string Email)
        {
            var authorToUpdate = await context.Authors.Where(a => a.Email == Email).FirstOrDefaultAsync();
            if (authorToUpdate != null)
            {
                if (!authorToUpdate.Status.Equals("OFFLINE"))
                {
                    authorToUpdate.Status = "OFFLINE";
                    context.Authors.Update(authorToUpdate);
                    await context.SaveChangesAsync();
                }

            }
        }

        public async Task<string?> GetAuthorStatusAsync(string Email)
        {
            var author = await context.Authors.Where(a => a.Email == Email).FirstOrDefaultAsync();
            if (author != null)
            {
                return author.Status;
            }
            else
            {
                return null;
            }
        }

        public async Task InsertAuthorAsync(string? Name, string Email, string Online)
        {
            Guid guid = Guid.NewGuid();
            GithubClaims githubclaims = new GithubClaims();
            if (Name != null)
            {
                await context.Authors.AddAsync(new Author() { AuthorId = guid.ToString(), Name = Name, Email = Email, Status = Online, Image = await githubclaims.GetGitHubClaimsUserImageAsync(Name) });
            }
        }

        // get all authors

    }
}