namespace Chirp.Core
{
    public interface IHashtagTextRepository
    {
        Task AddHashtag(string HashtagText);
        Task RemoveHashtag(string HashtagText);
        Task<List<string>> GetUniqueHashtagTextsAsync();
    }
}