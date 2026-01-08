using Microsoft.Extensions.Configuration;
using SampleProject.Infrastructure.Encryption;

// 讀取設定檔
var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "SampleProject.Api");
var builder = new ConfigurationBuilder()
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = builder.Build();

// 取得加密服務
var encryptionService = new Aes256EncryptionService(configuration);

// 取得加密後的連線字串
var encryptedConnectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(encryptedConnectionString))
{
    Console.WriteLine("錯誤：找不到 DefaultConnection 連線字串");
    return;
}

Console.WriteLine("加密後的連線字串：");
Console.WriteLine(encryptedConnectionString);
Console.WriteLine();

// 檢查是否有 encrypted: 前綴
if (encryptedConnectionString.StartsWith("encrypted:", StringComparison.OrdinalIgnoreCase))
{
    var encryptedValue = encryptedConnectionString.Substring("encrypted:".Length);

    try
    {
        // 解密
        var decrypted = encryptionService.Decrypt(encryptedValue);

        Console.WriteLine("✓ 解密成功！");
        Console.WriteLine("解密後的連線字串：");
        Console.WriteLine(decrypted);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ 解密失敗：{ex.Message}");
    }
}
else
{
    Console.WriteLine("連線字串未加密（沒有 encrypted: 前綴）");
}
