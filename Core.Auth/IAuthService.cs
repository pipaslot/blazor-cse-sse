using System.Threading.Tasks;

namespace Core.Auth
{
    public interface IAuthService
    {
        Task<string[]> GetUserPermissions();
    }
}
