namespace SampleProject.Infrastructure.Encryption;

/// <summary>
/// 加密/解密服務介面
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// 加密字串
    /// </summary>
    /// <param name="plainText">明文</param>
    /// <returns>Base64 編碼的加密字串</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// 解密字串
    /// </summary>
    /// <param name="cipherText">Base64 編碼的加密字串</param>
    /// <returns>解密後的明文</returns>
    string Decrypt(string cipherText);
}
