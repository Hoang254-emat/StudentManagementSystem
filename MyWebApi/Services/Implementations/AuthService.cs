using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims;         
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;     
using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.Entities;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class AuthService(DataContext context, IConfiguration configuration) : IAuthService
    {
        private readonly DataContext _context = context;
        private readonly IConfiguration _configuration = configuration; 

        public async Task<bool> RegisterStudentAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Id == dto.Id))
                return false;

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Id = dto.Id,
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                RoleId = 2
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterTeacherAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Id == dto.Id))
                return false;

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Id = dto.Id,
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                RoleId = 3
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RegisterAdminAsync(RegisterAdminDto dto)
        {
            using var hmac = new HMACSHA512();

            var user = new User
            { 
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                RoleId = 1
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }
        public async Task<string?> CheckLoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return null;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role.RoleName) 
            };

            var tokenKeySection = _configuration.GetSection("AppSettings:Token").Value;
            if (string.IsNullOrEmpty(tokenKeySection)) return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeySection));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), 
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); 
        }
    }
}