
using System.ComponentModel.DataAnnotations;
using System.Data;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class CheepRepository : ICheepRepository, IDisposable
    {
        private ChirpDBContext context;

        public CheepRepository(ChirpDBContext context)
        {
            this.context = context;
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

        IEnumerable<CheepDTO> ICheepRepository.GetCheeps(int page)
        {
            //minus by 1 so pages start from 1
            page = page - 1;
            var cheeps = context.Cheeps
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId))
                .ToList();
            return cheeps;
        }

        CheepDTO? ICheepRepository.GetCheepByID(int cheepId)
        {
            var cheep = context.Cheeps.Find(cheepId);
            if (cheep != null)
            {
                return new CheepDTO(cheep.CheepId, cheep.Text, cheep.TimeStamp, cheep.Author.Name, cheep.AuthorId);
            }
            else
            {
                return null;
            }
        }

        void ICheepRepository.InsertCheep(CheepDTO CheepDTO)
        {
            var author = context.Authors.Find(CheepDTO.AuthorId) ?? throw new Exception("Author could not be found by AuthorID");
            context.Cheeps.Add(new Cheep
            {
                CheepId = CheepDTO.Id,
                Text = CheepDTO.Message,
                TimeStamp = CheepDTO.TimeStamp,
                Author = author,
                AuthorId = author.AuthorId
            });
        }



        void ICheepRepository.DeleteCheep(int cheepId)
        {
            var cheep = context.Cheeps.Find(cheepId);
            if (cheep != null)
            {
                context.Cheeps.Remove(cheep);
            }
        }

        void ICheepRepository.UpdateCheep(CheepDTO Cheep)
        {
            var cheep = context.Cheeps.Find(Cheep.Id);
            var author = context.Authors.Find(Cheep.AuthorId);
            if (author == null)
            {
                throw new Exception("Failed to update Cheep. Failed to find author by the CheepDTO AuthorID");
            }
            if (cheep != null)
            {
                cheep.Text = Cheep.Message;
                cheep.TimeStamp = Cheep.TimeStamp;
                cheep.Author = author;
            }
            else
            {
                throw new Exception("Failed to Update cheep. Failed to find Cheep from provided CheepDTO");
            }
        }

        IEnumerable<CheepDTO> ICheepRepository.GetCheepsByAuthor(string authorName, int page)
        {
            //minus by 1 so pages start from 1
            page = page - 1;
            var cheeps = context.Cheeps
                .Where(c => c.Author.Name == authorName)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId))
                .ToList();
            return cheeps;
        }

        // for home page
        int ICheepRepository.getPages()
        {
            return (int)Math.Ceiling(context.Cheeps.Count() / 32.0);
        }

        // get pages for user timeline
        int ICheepRepository.getPagesUser(string author)
        {

            return (int)Math.Ceiling(context.Cheeps.Where(c => c.Author.Name == author).Count() / 32.0);
        }


    }
}