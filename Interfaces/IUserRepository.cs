using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> GetUserById(string id);
        bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(string id);
        bool Save();
    }
}
