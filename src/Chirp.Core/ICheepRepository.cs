using System.Threading.Tasks;

namespace Chirp.Core
{
    public interface ICheepRepository
    {
        Task<IEnumerable<CheepDTO>> GetCheepsAsync(int page);
        Task<CheepDTO?> GetCheepByIDAsync(string cheepId);


        Task<IEnumerable<CheepDTO>> GetCheepsByAuthorAsync(string authorName, int page);
        Task<IEnumerable<CheepDTO>> GetCheepsByAuthorsAsync(List<String> authorNames, int page);
        Task<IEnumerable<CheepDTO>> GetCheepsByCheepIdsAsync(List<String> authorNames, int page);

        Task<int> GetPagesAsync();
        Task<int> GetPagesUserAsync(string author);
        Task<int> GetPagesFromCheepCountAsync(int cheepCount);
        Task InsertCheepAsync(CheepDTO Cheep);
        Task DeleteCheepAsync(string cheepId);
        Task UpdateCheepAsync(CheepDTO Cheep);
    }
}