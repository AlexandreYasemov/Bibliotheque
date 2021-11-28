using Bibliotheque.DataAccess.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotheque.helpers
{
    public class Security
    {
        /// <summary>
        /// Check the username against the password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static public Account AuthantificateUser(string username, string password)
        {
            using (PhpfribiblioContext context = new PhpfribiblioContext())
            {
                Account user = context.Accounts.Where(c => c.Name == username).FirstOrDefault();
                if (user == null)
                    return null;

                if (user.Salt == null)
                    return user;

                if (AuthantificatePassword(user, password))
                    return user;
                else
                    return null;
            }
        }

        /// <summary>
        /// Salt and hash password to check against the password in the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static public bool AuthantificatePassword(Account user, string password)
        {
            if (user.Password == null || user.Salt == null)
                return true;

            Security crypto = new Security();
            byte[] hashedPassword = crypto.HashPassword(password, user.Salt);

            return hashedPassword.SequenceEqual(user.Password);
        }

        /// <summary>
        /// Generate random salt for password
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[255];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        /// <summary>
        /// Adds the salt then hash the password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public byte[] HashPassword(string password, byte[] salt)
        {

            byte[] bytePassword = Encoding.UTF8.GetBytes(password);
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[bytePassword.Length + salt.Length];

            for (int i = 0; i < bytePassword.Length; i++)
            {
                plainTextWithSaltBytes[i] = bytePassword[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[bytePassword.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
    }
}
