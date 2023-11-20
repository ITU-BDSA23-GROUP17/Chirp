using Chirp.Core;

public class ViewModel
{
    public IEnumerable<CheepDTO> Cheeps { get; set; }
    
    public int pageNr { get; set; }
    public int pages { get; set; }

}