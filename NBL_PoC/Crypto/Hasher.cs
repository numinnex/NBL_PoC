using System.Security.Cryptography;

public static class Hasher 
{
    public static string ComputeSha256Hash(int inputValue) 
    {
        byte[] inputBytes = BitConverter.GetBytes(inputValue);
        using (sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            byte[] truncatedHash = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, truncatedHash, 0, 16);
            string hashString = BitConverter.ToString(truncatedHash).Replace("-", "");

            Console.WriteLine("Original Integer: " + inputValue);
            Console.WriteLine("128-bit Hash: " + hashString);
            return hashString;
        }
    }
}