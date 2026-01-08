# AES256 連線字串加密/解密使用說明

## 概述

本專案實作了 AES256 加密/解密服務，用於保護敏感資訊（如資料庫連線字串）的安全性。

## 功能特點

- **AES256 加密**：使用 256 位元金鑰進行加密
- **CBC 模式**：使用 Cipher Block Chaining 模式
- **PKCS7 填充**：標準填充方式
- **Base64 編碼**：加密結果以 Base64 格式儲存，便於在設定檔中使用

## 設定步驟

### 1. 產生加密金鑰和 IV

使用 `EncryptionKeyGenerator` 工具類產生金鑰：

```csharp
using SampleProject.Infrastructure.Encryption;

// 方法 1：產生隨機金鑰（推薦用於生產環境）
var (key, iv) = EncryptionKeyGenerator.GenerateRandomKeys();
Console.WriteLine($"Key: {key}");
Console.WriteLine($"IV: {iv}");

// 方法 2：從密碼產生金鑰（用於開發環境，便於團隊共享）
var (key2, iv2) = EncryptionKeyGenerator.GenerateKeysFromPassword("YourSecurePassword");
Console.WriteLine($"Key: {key2}");
Console.WriteLine($"IV: {iv2}");
```

### 2. 設定 appsettings.json

在 `appsettings.json` 或 `appsettings.Production.json` 中加入加密設定：

```json
{
  "Encryption": {
    "Key": "你的Base64編碼金鑰（32 bytes）",
    "IV": "你的Base64編碼IV（16 bytes）"
  },
  "ConnectionStrings": {
    "DefaultConnection": "encrypted:你的加密後連線字串"
  }
}
```

### 3. 加密連線字串

使用以下程式碼加密連線字串：

```csharp
// 在 Program.cs 或工具程式中
var builder = WebApplication.CreateBuilder(args);
var encryptionService = new Aes256EncryptionService(builder.Configuration);

var plainConnectionString = "Server=localhost;Database=SampleProjectDb;User Id=sa;Password=aptx4869;TrustServerCertificate=True;";
var encrypted = encryptionService.Encrypt(plainConnectionString);
Console.WriteLine($"Encrypted: encrypted:{encrypted}");
```

### 4. 設定連線字串

將加密後的連線字串放入設定檔：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "encrypted:你的加密後連線字串"
  }
}
```

## 使用方式

### 自動解密

`SqlConnectionFactory` 會自動偵測連線字串是否以 `encrypted:` 開頭，如果是則自動解密：

```csharp
// 在 appsettings.json 中
"ConnectionStrings": {
  "DefaultConnection": "encrypted:Base64EncryptedString"
}

// SqlConnectionFactory 會自動處理
// 如果沒有 "encrypted:" 前綴，則直接使用原始字串
```

### 手動加密/解密

如果需要手動使用加密服務：

```csharp
public class MyService
{
    private readonly IEncryptionService _encryptionService;

    public MyService(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void EncryptData()
    {
        var plainText = "敏感資訊";
        var encrypted = _encryptionService.Encrypt(plainText);
        // 儲存 encrypted 到資料庫或設定檔
    }

    public void DecryptData(string encrypted)
    {
        var decrypted = _encryptionService.Decrypt(encrypted);
        // 使用 decrypted
    }
}
```

## 安全建議

### 生產環境

1. **使用環境變數**：將加密金鑰和 IV 存放在環境變數中，不要放在設定檔中
   ```json
   {
     "Encryption": {
       "Key": "%ENCRYPTION_KEY%",
       "IV": "%ENCRYPTION_IV%"
     }
   }
   ```

2. **使用 Azure Key Vault 或 AWS Secrets Manager**：將敏感資訊存放在安全的金鑰管理服務中

3. **定期輪換金鑰**：定期更換加密金鑰，並重新加密所有敏感資料

4. **限制存取權限**：確保只有授權的應用程式和人員可以存取加密金鑰

### 開發環境

1. **使用固定密碼**：團隊可以使用相同的密碼產生金鑰，便於共享設定檔
2. **不要提交敏感資訊**：確保 `.gitignore` 排除包含真實金鑰的設定檔

## 範例：產生金鑰的 Console 應用程式

建立一個簡單的 Console 應用程式來產生金鑰：

```csharp
using SampleProject.Infrastructure.Encryption;

Console.WriteLine("=== AES256 加密金鑰產生器 ===\n");

// 產生隨機金鑰
var (key, iv) = EncryptionKeyGenerator.GenerateRandomKeys();

Console.WriteLine("隨機產生的金鑰（用於生產環境）：");
Console.WriteLine($"Key: {key}");
Console.WriteLine($"IV: {iv}\n");

// 從密碼產生金鑰
Console.Write("輸入密碼（用於開發環境）：");
var password = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(password))
{
    var (key2, iv2) = EncryptionKeyGenerator.GenerateKeysFromPassword(password);
    Console.WriteLine($"\n從密碼產生的金鑰：");
    Console.WriteLine($"Key: {key2}");
    Console.WriteLine($"IV: {iv2}");
}
```

## 注意事項

1. **金鑰管理**：加密金鑰和 IV 必須妥善保管，遺失將無法解密資料
2. **金鑰長度**：Key 必須是 32 bytes（256 bits），IV 必須是 16 bytes（128 bits）
3. **Base64 編碼**：金鑰和 IV 在設定檔中使用 Base64 編碼格式
4. **連線字串格式**：加密的連線字串必須以 `encrypted:` 前綴開頭
5. **錯誤處理**：如果解密失敗，會拋出 `CryptographicException`

## 疑難排解

### 問題：解密失敗

**可能原因**：
- 金鑰或 IV 設定錯誤
- 加密字串格式不正確（不是 Base64）
- 使用了不同的金鑰加密和解密

**解決方法**：
1. 確認 `Encryption:Key` 和 `Encryption:IV` 設定正確
2. 確認加密字串是有效的 Base64 格式
3. 確認加密和解密使用相同的金鑰和 IV

### 問題：連線字串無法解密

**可能原因**：
- 連線字串沒有 `encrypted:` 前綴
- `IEncryptionService` 未註冊

**解決方法**：
1. 確認連線字串格式：`encrypted:Base64String`
2. 確認在 `Program.cs` 中已註冊 `IEncryptionService`
