using MyWebApi.DTOs;
using MyWebApi.Entities;

namespace MyWebApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterStudentAsync(RegisterDto dto);
        Task<bool> RegisterTeacherAsync(RegisterDto dto);
        Task<bool> RegisterAdminAsync(RegisterAdminDto dto);
        Task<bool> UserExistAsync(string username);
        Task<string?> CheckLoginAsync(LoginDto dto);
    }
}
