using System;
using System.Security.Cryptography;
using System.Text;

namespace TrainingProject.DomainLogic.Helpers
{
    public static class HashGenerator
    {
        public static string Encrypt(string str)
        {
            var sha256 = new SHA256Managed();
            var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)));
            return hash;
        }
    }
}
