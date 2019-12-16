using System.Threading.Tasks;

namespace App.Shared
{
    public interface IConfigProvider
    {
        Task<Config> GetConfig();
    }
}