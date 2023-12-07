
namespace Chirp.Core
{
    public interface IUserService
    {

        Task DeleteUserById(string? userId);
        Task ListUsers();
    }
}