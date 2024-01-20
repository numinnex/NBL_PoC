using System.Security.Cryptography;

namespace NBL_PoC_Api.Crypto;

public static class Hasher 
{
    public static string Compute128BitHash(int id) 
    {
        var inputBytes = BitConverter.GetBytes(id);
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(inputBytes);
            var truncatedHash= new byte[16];
            Array.Copy(hashBytes, truncatedHash, 16);
            var hashString = BitConverter.ToString(truncatedHash).Replace("-", "").ToLower();

            return hashString;
        }
    }
}