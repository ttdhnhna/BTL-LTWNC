using BTL_LTWNC.Models;
using System.Collections.Generic;

namespace BTL_LTWNC.Repository
{
    public interface IUserRepository
    {
        bool Register(User user);
        User Login(string email, string password);
        User GetByEmail(string email);
        bool EmailExists(string email);

        // Admin functions
        List<User> GetAllUsers();
        bool UpdateUser(User updatedUser, string newPassword = null); 
        bool DeleteUser(int userId);
        bool AddUser(User user);
        List<User> SearchUsers(string keyword);
        User GetUserById(int userId); 
        
        // User functions
        // User GetByEmail(string email);
        bool UpdateNewPassUser(User user, string newPassword);

    }
}
