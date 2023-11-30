namespace Chirp.Core
{
    public interface IHashtagRepository
    {
        Task<List<string>> GetCheepIDsByHashtag(string Hashtag);
        Task InsertNewHashtagFromCheep(string Hashtag, string CheepID);
        Task RemoveHashtag(string Hashtag);
    }
}