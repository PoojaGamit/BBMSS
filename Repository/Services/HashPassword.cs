
using BBMSDATA1.Models;
using BBMSDATA1.Repository.Generic.Interface;
using Microsoft.AspNetCore.Identity;

namespace BBMSDATA1.Repository.Services
{
    public class HashPassword
    {
        private readonly IRepo<Users> userrepo;
        private readonly PasswordHasher<Users> passwordHasher;
        public HashPassword(IRepo<Users> userrepo)
        {
            this.userrepo = userrepo;
            passwordHasher = new PasswordHasher<Users>();

        }
        public async Task RegisterUserAsync(Users user, string password)
        { 
            user.PasswordHash=passwordHasher.HashPassword(user,password);
            await userrepo.AddAsync(user);
        
        }
        public async Task<bool> VerifyPasswordAsync(Users user, string enteredPassword)
        {
            return await Task.Run(() =>
            {
                if (user.PasswordHash == null)
                {
                    return false;
                }

                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, enteredPassword);

                return result == PasswordVerificationResult.Success;
            });
        }

    }
}
