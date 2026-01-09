using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SampleProject.Infrastructure.Encryption;

/// <summary>
/// AES256 加密/解密提供者實作
/// </summary>
public class Aes256EncryptionProvider : IEncryptionProvider
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public Aes256EncryptionProvider(IConfiguration configuration)
    {
        var encryptionKey = configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption:Key is not configured.");

        var encryptionIv = configuration["Encryption:IV"]
            ?? throw new InvalidOperationException("Encryption:IV is not configured.");

        // 驗證金鑰長度（AES256 需要 32 bytes）
        _key = ConvertKeyToBytes(encryptionKey, 32);
        _iv = ConvertKeyToBytes(encryptionIv, 16);
    }

    /// <summary>
    /// 將字串轉換為指定長度的位元組陣列
    /// </summary>
    private static byte[] ConvertKeyToBytes(string key, int requiredLength)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        // 如果已經是 Base64 編碼，先解碼
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(key);
        }
        catch
        {
            // 如果不是 Base64，使用 UTF-8 編碼
            bytes = Encoding.UTF8.GetBytes(key);
        }

        // 如果長度不符合要求，使用雜湊來產生固定長度
        if (bytes.Length != requiredLength)
        {
            if (requiredLength == 32)
            {
                // 使用 SHA256 產生 32 bytes
                using var sha256 = SHA256.Create();
                bytes = sha256.ComputeHash(bytes);
            }
            else
            {
                // 使用 SHA1 產生 20 bytes，然後擴展到 requiredLength
                using var sha1 = SHA1.Create();
                var hash = sha1.ComputeHash(bytes);
                using var pbkdf2 = new Rfc2898DeriveBytes(key, hash, 10000, HashAlgorithmName.SHA256);
                bytes = pbkdf2.GetBytes(requiredLength);
            }

            // 如果還是太長，截取前 requiredLength 個位元組
            if (bytes.Length > requiredLength)
            {
                var result = new byte[requiredLength];
                Array.Copy(bytes, result, requiredLength);
                bytes = result;
            }
            // 如果太短，使用 PBKDF2 來擴展
            else if (bytes.Length < requiredLength)
            {
                using var pbkdf2 = new Rfc2898DeriveBytes(key, bytes, 10000, HashAlgorithmName.SHA256);
                bytes = pbkdf2.GetBytes(requiredLength);
            }
        }

        return bytes;
    }

    /// <summary>
    /// 加密字串
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return plainText;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    /// <summary>
    /// 解密字串
    /// </summary>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return cipherText;
        }

        try
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipherBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch (FormatException)
        {
            throw new CryptographicException("Invalid cipher text format. Expected Base64 encoded string.");
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException("Decryption failed. Please check the encryption key and IV.", ex);
        }
    }
}
