using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BDA.DataModel
{
    public static class LibFunction
    {
       

        public static DbContext GetDbContextFromEntity(object entity)
        {
            var object_context = GetObjectContextFromEntity(entity);

            if (object_context == null || object_context.TransactionHandler == null)
                return null;

            return object_context.TransactionHandler.DbContext;
        }

        private static ObjectContext GetObjectContextFromEntity(object entity)
        {
            var field = entity.GetType().GetField("_entityWrapper");

            if (field == null)
                return null;

            var wrapper = field.GetValue(entity);
            var property = wrapper.GetType().GetProperty("Context");
            var context = (ObjectContext)property.GetValue(wrapper, null);

            return context;
        }
     
        public static string GenerateHash(string SourceText)
        {
            //Create an encoding object to ensure the encoding standard for the source text
            UnicodeEncoding Ue = new UnicodeEncoding();
            //Retrieve a byte array based on the source text
            byte[] ByteSourceText = Ue.GetBytes(SourceText);
            //Instantiate an MD5 Provider object
            MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
            //Compute the hash value from the source
            byte[] ByteHash = Md5.ComputeHash(ByteSourceText);
            //And convert it to String format for return
            return Convert.ToBase64String(ByteHash);
        }
        public static string CreateRandomPin(int length = 8)
        {
            //return "1234";
            const string valid = "0123456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static string CreateRandomKeyCode(int length = 8)
        {
            const string valid = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static string CreateRandomPassword(int length = 8)
        {
            const string valid = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789*$-+?_!{}/";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static string HashPasswordPbkdf2(string pwd, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                             password: pwd,
                             salt: Encoding.UTF8.GetBytes(salt),
                             prf: KeyDerivationPrf.HMACSHA512,
                             iterationCount: 10000,
                             numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public static string HashPasswordSHA256(string pwd)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        // return how much time passed since date object
        public static string GetTimeSince(DateTime objDateTime)
        {
            // here we are going to subtract the passed in DateTime from the current time converted to UTC
            TimeSpan ts = DateTime.Now.Subtract(objDateTime);
            int intDays = ts.Days;
            int intHours = ts.Hours;
            int intMinutes = ts.Minutes;
            int intSeconds = ts.Seconds;

            if (intDays > 0)
                return string.Format("{0} hari lalu", intDays);

            if (intHours > 0)
                return string.Format("{0} jam lalu", intHours);

            if (intMinutes > 0)
                return string.Format("{0} menit lalu", intMinutes);

            if (intSeconds > 0)
                return string.Format("{0} detik lalu", intSeconds);

            // let's handle future times..just in case
            if (intDays < 0)
                return string.Format("dalam {0} hari", Math.Abs(intDays));

            if (intHours < 0)
                return string.Format("dalam {0} jam", Math.Abs(intHours));

            if (intMinutes < 0)
                return string.Format("dalam {0} menit", Math.Abs(intMinutes));

            if (intSeconds < 0)
                return string.Format("dalam {0} detik", Math.Abs(intSeconds));

            return "barusan";
        }


    }
}
