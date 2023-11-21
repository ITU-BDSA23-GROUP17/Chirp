
namespace Chirp.Core
{
    public interface ICheepRepository : IDisposable
    {
        public IEnumerable<CheepDTO> GetCheeps(int page);
        CheepDTO? GetCheepByID(int cheepId);
        IEnumerable<CheepDTO>? GetCheepsByAuthor(string authorName, int page);
        IEnumerable<CheepDTO>? GetCheepsByAuthors(List<String> authorNames, int page);

        int getPages();
        int getPagesUser(string author);
        void InsertCheep(CheepDTO Cheep);
        void DeleteCheep(int cheepId);
        void UpdateCheep(CheepDTO Cheep);

        void Save();

    }
}