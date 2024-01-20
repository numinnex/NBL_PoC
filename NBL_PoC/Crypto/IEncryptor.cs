namespace NBL_PoC_Api.Crypto;

public interface IEncryptor 
{
    public string Encrypt(string value); 
    public string Decrypt(string value);
}