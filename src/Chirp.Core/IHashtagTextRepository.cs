namespace Chirp.Core
{
    public interface IHashtagTextRepository
    {
        Task<List<string>> GetUniqueHashtagTextsAsync();
    }
}