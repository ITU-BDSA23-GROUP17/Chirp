
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
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name))
                .ToList();
            return cheeps;
        }

        CheepDTO ICheepRepository.GetCheepByID(int cheepId)
        {
            var cheep = context.Cheeps.Find(cheepId);
            return new CheepDTO(cheep.CheepId, cheep.Text, cheep.TimeStamp, cheep.Author.Name);
        }

        void ICheepRepository.InsertCheep(CheepDTO Cheep)
        {
            context.Cheeps.Add(new Cheep
            {
                Text = Cheep.Message,
                TimeStamp = Cheep.TimeStamp,
                Author = context.Authors.Find(Cheep.Author)
            });
        }

        void ICheepRepository.DeleteCheep(int cheepId)
        {
            context.Cheeps.Remove(context.Cheeps.Find(cheepId));
        }

        void ICheepRepository.UpdateCheep(CheepDTO Cheep)
        {
            var cheep = context.Cheeps.Find(Cheep.Id);
            cheep.Text = Cheep.Message;
            cheep.TimeStamp = Cheep.TimeStamp;
            cheep.Author = context.Authors.Find(Cheep.Author);
        }

        IEnumerable<CheepDTO> ICheepRepository.GetCheepsByAuthor(string authorName, int page)
        {
            var cheeps = context.Cheeps
                .Where(c => c.Author.Name == authorName)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 32)
                .Take(32)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name))
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