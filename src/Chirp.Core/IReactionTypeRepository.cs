
namespace Chirp.Core
{
    public interface IReactionTypeRepository
    {

        string GetReactionTypeNameById(string reactionTypeId);

        string GetReactionTypeIconById(string reactionTypeId);

    }
}