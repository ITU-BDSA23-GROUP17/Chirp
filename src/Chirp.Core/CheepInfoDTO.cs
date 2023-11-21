namespace Chirp.Core;
public record CheepInfoDTO(string Message, DateTime TimeStamp, string AuthorName, bool UserIsFollowingAuthor);
//Why is this called DTO...?