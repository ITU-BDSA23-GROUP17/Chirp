
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
            var cheeps = context.Cheeps
                .OrderByDescending(c => c.TimeStamp)
                .Skip(page * 10)
                .Take(10)
                .Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.AuthorId ))
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
            throw new NotImplementedException();
        }

        void ICheepRepository.DeleteCheep(int cheepId)
        {
            context.Cheeps.Remove(context.Cheeps.Find(cheepId));
        }

        void ICheepRepository.UpdateCheep(CheepDTO Cheep)
        {
            throw new NotImplementedException();
        }
    }
}