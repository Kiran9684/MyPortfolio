using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MyPortfolio.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Business.Utilities
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly ILogger<PasswordHasher> _logger;

        public PasswordHasher(ILogger<PasswordHasher> logger)
        {
            this._logger = logger;
        }

        private byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public HashPasswordInfo HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt();
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length]; //empty array to hold concatenated password and salt
            byte[] hashBytes;

            // Concatenate password and salt
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt,0,saltedPassword, passwordBytes.Length, salt.Length);

            using(SHA256 sha256 = SHA256.Create())
            {
                // Hash the concatenated password and salt
                hashBytes = sha256.ComputeHash(saltedPassword);
            }

            return new HashPasswordInfo()
            {
                HashedPassword = Convert.ToBase64String(hashBytes),
                Salt = Convert.ToBase64String(salt)
            }; 
        }

        public bool VerifyPassword(HashPasswordInfo hashedPasswordWithSalt, string providedPassword)
        {
            byte[] providedPassBytes = Encoding.UTF8.GetBytes(providedPassword);
            byte[] salt = Convert.FromBase64String(hashedPasswordWithSalt.Salt);

            byte[] saltedPass = new byte[providedPassBytes.Length + salt.Length];
            byte[] hashBytes;

            Buffer.BlockCopy(providedPassBytes,0, saltedPass,0, providedPassBytes.Length);
            Buffer.BlockCopy(salt,0, saltedPass, providedPassBytes.Length, salt.Length);

            using (SHA256 sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(saltedPass);
            }

            if(Convert.ToBase64String(hashBytes) == hashedPasswordWithSalt.HashedPassword)
            {
                _logger.LogWarning("Password verification success.");
                return true;
            }
            else
            {
                _logger.LogWarning("Password verification failed.");
                return false;
            }
        }
    }
}
