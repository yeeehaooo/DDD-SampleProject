using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using SampleProject.Infrastructure.Encryption;

// 讀取設定檔
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = builder.Build();

// 取得加密服務
var encryptionService = new Aes256EncryptionService(configuration);

// 取得原始連線字串
var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("錯誤：找不到 DefaultConnection 連線字串");
    return;
}

Console.WriteLine("原始連線字串：");
Console.WriteLine(connectionString);
Console.WriteLine();

// 加密連線字串
var encrypted = encryptionService.Encrypt(connectionString);
var encryptedConnectionString = $"encrypted:{encrypted}";

Console.WriteLine("加密後的連線字串：");
Console.WriteLine(encryptedConnectionString);
Console.WriteLine();

Console.WriteLine("請將以下內容更新到 appsettings.json：");
Console.WriteLine($"\"ConnectionStrings\": {{");
Console.WriteLine($"  \"DefaultConnection\": \"{encryptedConnectionString}\",");
Console.WriteLine($"  \"Redis\": \"{configuration.GetConnectionString("Redis")}\"");
Console.WriteLine($"}}");
