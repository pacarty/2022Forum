using Core.Models;

namespace Core.Interfaces
{
    public interface ISubforumService
    {
        Task<List<SubforumModel>> GetSubforumModels();
    }
}
