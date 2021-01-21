using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StudentForum.BusinessLogic
{
    internal class PasswordHasher
    {
        private const int IterationCount = 10000;
        private const int HashLength = 256 / 8;
        private const int SaltLength = 128 / 8;

        public string CreateSalt()
        {
            byte[] salt = new byte[SaltLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public string CreatePassworHash(string password, string salt)
        {
            return Convert.ToBase64String(Hash(password, salt));
        }

        private byte[] Hash(string password, string salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashLength);
        }

        public bool ComparePassword(string password, string passwordHash, string salt)
        {
            var array1 = Hash(password, salt);
            var array2 = Convert.FromBase64String(passwordHash);

            var result = true;
            for(int i = 0; i< array2.Length; i++)
            {
                result &= array1[i] == array2[i];
            }
            return result;
        }


    }
}
