namespace Chirp.Core
{
    public record CheepInfoDTO
    {
        public CheepDTO Cheep { get; init; }
        public bool UserIsFollowingAuthor { get; init; }

        public bool UserReactToCheep {get; set;}
        public required string TotalReactions {get; set;}
    }
}
