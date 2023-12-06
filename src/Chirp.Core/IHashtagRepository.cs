namespace Chirp.Core
{
    public interface IHashtagRepository
    {
        Task<List<string>> GetCheepIDsByHashtagTextAsync(string Hashtag);
        Task InsertNewHashtagCheepPairingAsync(string Hashtag, string CheepID);
        Task RemoveHashtag(string Hashtag);
    }
}