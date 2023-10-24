
namespace Chirp.Core
{
    public interface ICheepRepository : IDisposable
    {
        IEnumerable<CheepDTO> GetCheeps(int page);
        CheepDTO GetCheepByID(int cheepId);
        IEnumerable<CheepDTO> GetCheepsByAuthor(string authorName, int page);

        int getPages();
        int getPagesUser(string author);
        void InsertCheep(CheepDTO Cheep);
        void DeleteCheep(int cheepId);
        void UpdateCheep(CheepDTO Cheep);
        void Save();

    }
}