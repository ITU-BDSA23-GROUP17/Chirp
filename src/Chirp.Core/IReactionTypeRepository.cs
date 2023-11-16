
namespace Chirp.Core
{
    public interface IReactionTypeRepository : IDisposable
    {

        string GetReactionTypeNameById(string reactionTypeId);

        string GetReactionTypeIconById(string reactionTypeId);

    }
}