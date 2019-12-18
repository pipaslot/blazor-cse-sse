using System.Threading.Tasks;

namespace App.Shared
{
    public interface IAuthService
    {
        Task SignIn(string username, string password);
        Task SignOut();
    }
}
