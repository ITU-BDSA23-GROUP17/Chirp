
namespace Chirp.Core
{
    public interface IReactionTypeRepository : IDisposable
    {

        void GetReactionTypeById(string reactionTypeId);

    }
}