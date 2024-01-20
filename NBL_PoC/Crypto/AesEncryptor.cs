using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

public class AesEncryptor : IEncryptor
{
    private readonly AesSettings _aesSettings;
    public AesEncryptor(IOptions<AesSettings> aesSettings)
    {
        _aesSettings = aesSettings.Value;
    }
    public string Encrypt(string value) 
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(_aesSettings.Secret);
        aesAlg.IV = Encoding.UTF8.GetBytes(_aesSettings.IV);

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using MemoryStream msEncrypt = new MemoryStream();
        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using StreamWriter swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(value);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }
    public string Decrypt(string value)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(_aesSettings.Secret);
        aesAlg.IV = Encoding.UTF8.GetBytes(_aesSettings.IV);

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(value));
        using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}