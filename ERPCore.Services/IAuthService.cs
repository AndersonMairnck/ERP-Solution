using ERPCore.Models;

namespace ERPCore.Services
{
    public interface IAuthService
    {
        Task<User> Authenticate(string username, string password);
        Task<bool> Register(User user, string password);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
        string GenerateJwtToken(User user);
    }
}