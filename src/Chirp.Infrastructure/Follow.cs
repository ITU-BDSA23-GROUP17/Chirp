namespace Chirp.Infrastructure;

public class Follow
{

    public required string FollowerId { get; set; }
    public required string FollowingId { get; set; }
    public required DateTime Timestamp { get; set; }

}
