using System.Data;
using Chirp.Core;
using Microsoft.EntityFrameworkCore;


/*
    This warning tells us that await methods lacks inside 
    the methods, but these methods are later used by
    asynchronous methods. Thus, we still need to declare 
    them as async. 
*/
#pragma warning disable CS1998

namespace Chirp.Infrastructure
{
    public class CheepRepository : ICheepRepository
    {
        private ChirpDBContext context;

        public CheepRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CheepDTO>> GetCheepsAsync(int page)
        {
            //minus by 1 so pages start from 1
            page = page - 1;
            var cheeps = context.Cheeps
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image))
                .ToList();

            return cheeps;
        }

        public async Task<CheepDTO?> GetCheepByIDAsync(string cheepId)
        {
            var cheep = await context.Cheeps
                .Where(c => c.CheepId == cheepId)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image))
                .FirstOrDefaultAsync();

            return cheep;
        }


        public async Task<IEnumerable<CheepDTO>> GetCheepsByAuthorAsync(string authorName, int page)
        {
            //minus by 1 so pages start from 1
            page = page - 1;
            var cheeps = context.Cheeps
                .Where(c => c.Author.Name == authorName)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image))
                .ToList();

            return cheeps;
        }

        public async Task<IEnumerable<CheepDTO>> GetCheepsByAuthorsAsync(List<String> authorNames, int page)
        {
            //minus by 1 so pages start from 1
            page = page - 1;
            var cheeps = context.Cheeps
                .Where(c => authorNames.Contains(c.Author.Name))
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image))
                .ToList();

            return cheeps;
        }

        public async Task<IEnumerable<CheepDTO>> GetCheepsByCheepIdsAsync(List<String> cheepIds, int page)
        {
            //minus by 1 so pages start from 1
            page = page - 1;
            var cheeps = context.Cheeps
                .Where(c => cheepIds.Contains(c.CheepId))
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image))
                .ToList();

            return cheeps;
        }

        public async Task<int> GetPagesAsync()
        {
            return (int)Math.Ceiling(context.Cheeps.Count() / 32.0);

        }

        public async Task<int> GetPagesUserAsync(string author)
        {
            return (int)Math.Ceiling(context.Cheeps.Where(c => c.Author.Name == author).Count() / 32.0);

        }

        public async Task<int> GetPagesFromCheepCountAsync(int cheepCount)
        {
            return (int)Math.Ceiling(cheepCount / 32.0);

        }

        public async Task InsertCheepAsync(CheepDTO Cheep)
        {
            var author = await context.Authors.FindAsync(Cheep.AuthorId) ?? throw new Exception("Author could not be found by AuthorID");
            context.Cheeps.Add(new Cheep
            {
                CheepId = Cheep.Id,
                Text = Cheep.Message,
                TimeStamp = Cheep.TimeStamp,
                Author = author,
                AuthorId = author.AuthorId
            });
            await SaveAsync();
        }

        public async Task DeleteCheepAsync(string cheepId)
        {
            var cheep = await context.Cheeps.FindAsync(cheepId) ?? throw new Exception("Cheep could not be found by CheepID");
            context.Cheeps.Remove(cheep);
            await SaveAsync();
        }

        public async Task UpdateCheepAsync(CheepDTO Cheep)
        {
            var cheep = await context.Cheeps.FindAsync(Cheep.Id) ?? throw new Exception("Cheep could not be found by CheepID");
            cheep.Text = Cheep.Message;
            cheep.TimeStamp = Cheep.TimeStamp;
            await SaveAsync();
        }

    }
}