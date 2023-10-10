using Chirp.Objects;

namespace cheepRepoLibrary
{
    public interface ICheepRepository : IDisposable
    {
        IEnumerable<Cheep> GetCheeps();
        Cheep GetCheepByID(int cheepId);
        void InsertCheep(Cheep Cheep);
        void DeleteCheep(int cheepId);
        void UpdateCheep(Cheep Cheep);
        void Save();

    }
}