using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using NBL_PoC_Api.Options;

namespace NBL_PoC_Api.Crypto;

public sealed class AesEncryptor : IEncryptor
{
    private readonly AesSettings _aesSettings;
    public AesEncryptor(IOptions<AesSettings> aesSettings)
    {
        _aesSettings = aesSettings.Value;
    }
    public string Encrypt(string value) 
    {
        using var aesAlg = Aes.Create();
    
        var keyBytes = Encoding.UTF8.GetBytes(_aesSettings.Secret);
        aesAlg.Key = keyBytes;
    
        aesAlg.GenerateIV();
    
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
    
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(value);
        }
    
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string value)
    {
        using var aesAlg = Aes.Create();
    
        var keyBytes = Encoding.UTF8.GetBytes(_aesSettings.Secret);
        aesAlg.Key = keyBytes;
    
        var iv = new byte[aesAlg.BlockSize / 8];
        Array.Copy(Convert.FromBase64String(value), iv, iv.Length);
        aesAlg.IV = iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(Convert.FromBase64String(value).Skip(iv.Length).ToArray());
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }

}