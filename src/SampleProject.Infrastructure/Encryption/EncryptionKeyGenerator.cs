using System.Security.Cryptography;
using System.Text;

namespace SampleProject.Infrastructure.Encryption;

/// <summary>
/// 加密金鑰產生器工具類
/// </summary>
public static class EncryptionKeyGenerator
{
    /// <summary>
    /// 產生 AES256 加密所需的金鑰和 IV
    /// </summary>
    /// <param name="password">可選的密碼，用於產生金鑰（如果為 null，則使用隨機產生）</param>
    /// <returns>包含 Key 和 IV 的元組（Base64 編碼）</returns>
    public static (string Key, string IV) GenerateKeys(string? password = null)
    {
        byte[] keyBytes;
        byte[] ivBytes;

        if (!string.IsNullOrWhiteSpace(password))
        {
            // 使用 PBKDF2 從密碼產生固定長度的金鑰和 IV
            var salt = Encoding.UTF8.GetBytes("SampleProjectSalt2024"); // 固定 Salt（生產環境應使用隨機 Salt）

            using var keyDerivation = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            keyBytes = keyDerivation.GetBytes(32); // AES256 需要 32 bytes

            using var ivDerivation = new Rfc2898DeriveBytes(password + "IV", salt, 100000, HashAlgorithmName.SHA256);
            ivBytes = ivDerivation.GetBytes(16); // IV 需要 16 bytes
        }
        else
        {
            // 使用隨機產生
            keyBytes = new byte[32];
            ivBytes = new byte[16];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(keyBytes);
            rng.GetBytes(ivBytes);
        }

        return (Convert.ToBase64String(keyBytes), Convert.ToBase64String(ivBytes));
    }

    /// <summary>
    /// 產生隨機金鑰和 IV（用於生產環境）
    /// </summary>
    public static (string Key, string IV) GenerateRandomKeys()
    {
        return GenerateKeys(null);
    }

    /// <summary>
    /// 從密碼產生金鑰和 IV（用於開發環境，便於團隊共享）
    /// </summary>
    public static (string Key, string IV) GenerateKeysFromPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        return GenerateKeys(password);
    }
}
