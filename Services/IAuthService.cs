using GameAccountStore.DTOs;
using GameAccountStore.Helpers;
using GameAccountStore.Models;

namespace GameAccountStore.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<bool> UserExists(string username);
        string CreateToken(User user);
    }
}
