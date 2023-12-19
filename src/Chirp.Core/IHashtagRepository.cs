namespace Chirp.Core
{
    public interface IHashtagRepository
    {
        List<string> GetCheepIDsByHashtagText(string Hashtag);
        Task InsertNewHashtagCheepPairingAsync(string Hashtag, string CheepID);
        Task RemoveHashtagAsync(string Hashtag);
        List<string> GetPopularHashtags(List<string> hashtags);
    }
}