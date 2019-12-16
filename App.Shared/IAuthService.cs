using System.Threading.Tasks;

namespace App.Shared
{
    public interface IAuthService
    {
        Task<string[]> GetUserPermissions();
    }
}
