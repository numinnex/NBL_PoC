using System.Security.Cryptography;

namespace NBL_PoC_Api.Crypto;

public static class Hasher 
{
    public static string Compute128BitHash(int id)
    {
        var inputBytes = BitConverter.GetBytes(id);
        var hashBytes = SHA256.HashData(inputBytes);
        var truncatedHash = new byte[16];
        Array.Copy(hashBytes, truncatedHash, 16);
        var hashString = BitConverter.ToString(truncatedHash).Replace("-", "").ToLower();

        return hashString;
    }
}