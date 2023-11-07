using System.Security.AccessControl;

namespace Chirp.Core
{
    public interface IAuthorRepository : IDisposable
    {
        AuthorDTO? GetAuthorByName(string? name);

        AuthorDTO? GetAuthorByEmail(string email);

        AuthorDTO? GetAuthorById(string authorId);

        void InsertAuthor(string? Name, string Email);
        void DeleteAuthor(int authorId);
        void UpdateAuthor(AuthorDTO author);
        //var c9 = new Cheep() { CheepId = 9, AuthorId = a10.AuthorId, Author = a10, Text = "The folk on trust in me!", TimeStamp = DateTime.Parse("2023-08-01 13:15:30") };
    }
}