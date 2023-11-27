namespace Chirp.Core;
public record AuthorDTO(string AuthorId, string Name, string Email, string Status, List<CheepDTO> Cheeps);