using System.Security.Cryptography;

public static class Hasher 
{
    public static string Compute128BitHash(int id) 
    {
        byte[] inputBytes = BitConverter.GetBytes(id);
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            byte[] truncatedHash= new byte[16];
            Array.Copy(hashBytes, truncatedHash, 16);
            string hashString = BitConverter.ToString(truncatedHash).Replace("-", "").ToLower();

            return hashString;
        }
    }
}