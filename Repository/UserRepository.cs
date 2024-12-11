using Microsoft.EntityFrameworkCore;
using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(AppUser user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        // Retrieves all users from the database asynchronously
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // Retrieves a user by ID including their address from the database asynchronously
        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == id);
        }

        // Saves changes to the database
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        // Updates a user in the database
        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
