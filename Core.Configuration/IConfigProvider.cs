using System.Threading.Tasks;

namespace Core.Configuration
{
    public interface IConfigProvider
    {
        Task<Config> GetConfig();
    }
}