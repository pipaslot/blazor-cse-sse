using System.Threading.Tasks;

namespace Components.Services
{
    public interface IConfigProvider
    {
        Task<Config> GetConfig();
    }
}