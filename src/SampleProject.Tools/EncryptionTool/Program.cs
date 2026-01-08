using System.Security.Cryptography;
using System.Text;
using SampleProject.Infrastructure.Encryption;

Console.WriteLine("=== AES256 加密工具 ===\n");

while (true)
{
    Console.WriteLine("請選擇操作：");
    Console.WriteLine("1. 產生加密金鑰和 IV");
    Console.WriteLine("2. 加密連線字串");
    Console.WriteLine("3. 解密連線字串");
    Console.WriteLine("4. 退出");
    Console.Write("\n請輸入選項 (1-4): ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            GenerateKeys();
            break;
        case "2":
            EncryptConnectionString();
            break;
        case "3":
            DecryptConnectionString();
            break;
        case "4":
            Console.WriteLine("再見！");
            return;
        default:
            Console.WriteLine("無效的選項，請重新選擇。\n");
            break;
    }

    Console.WriteLine();
}

static void GenerateKeys()
{
    Console.WriteLine("\n--- 產生加密金鑰和 IV ---");
    Console.WriteLine("1. 隨機產生（推薦用於生產環境）");
    Console.WriteLine("2. 從密碼產生（用於開發環境）");
    Console.Write("請選擇 (1-2): ");

    var option = Console.ReadLine();

    if (option == "1")
    {
        var (key, iv) = EncryptionKeyGenerator.GenerateRandomKeys();
        Console.WriteLine("\n產生的金鑰：");
        Console.WriteLine($"Key: {key}");
        Console.WriteLine($"IV: {iv}");
        Console.WriteLine("\n請將以下內容加入 appsettings.json：");
        Console.WriteLine($"\"Encryption\": {{");
        Console.WriteLine($"  \"Key\": \"{key}\",");
        Console.WriteLine($"  \"IV\": \"{iv}\"");
        Console.WriteLine($"}}");
    }
    else if (option == "2")
    {
        Console.Write("請輸入密碼: ");
        var password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("密碼不能為空！");
            return;
        }

        var (key, iv) = EncryptionKeyGenerator.GenerateKeysFromPassword(password);
        Console.WriteLine("\n產生的金鑰：");
        Console.WriteLine($"Key: {key}");
        Console.WriteLine($"IV: {iv}");
        Console.WriteLine("\n請將以下內容加入 appsettings.json：");
        Console.WriteLine($"\"Encryption\": {{");
        Console.WriteLine($"  \"Key\": \"{key}\",");
        Console.WriteLine($"  \"IV\": \"{iv}\"");
        Console.WriteLine($"}}");
    }
    else
    {
        Console.WriteLine("無效的選項！");
    }
}

static void EncryptConnectionString()
{
    Console.WriteLine("\n--- 加密連線字串 ---");
    Console.Write("請輸入要加密的連線字串: ");
    var plainText = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(plainText))
    {
        Console.WriteLine("連線字串不能為空！");
        return;
    }

    Console.Write("請輸入加密金鑰 (Base64): ");
    var keyBase64 = Console.ReadLine();

    Console.Write("請輸入 IV (Base64): ");
    var ivBase64 = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(keyBase64) || string.IsNullOrWhiteSpace(ivBase64))
    {
        Console.WriteLine("金鑰和 IV 不能為空！");
        return;
    }

    try
    {
        // 建立臨時設定
        var configuration = new Dictionary<string, string?>
        {
            { "Encryption:Key", keyBase64 },
            { "Encryption:IV", ivBase64 }
        };

        var configBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(configuration);
        var config = configBuilder.Build();

        var encryptionService = new Aes256EncryptionService(config);
        var encrypted = encryptionService.Encrypt(plainText);

        Console.WriteLine("\n加密成功！");
        Console.WriteLine($"加密後的連線字串: encrypted:{encrypted}");
        Console.WriteLine("\n請將以下內容加入 appsettings.json：");
        Console.WriteLine($"\"ConnectionStrings\": {{");
        Console.WriteLine($"  \"DefaultConnection\": \"encrypted:{encrypted}\"");
        Console.WriteLine($"}}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"加密失敗: {ex.Message}");
    }
}

static void DecryptConnectionString()
{
    Console.WriteLine("\n--- 解密連線字串 ---");
    Console.Write("請輸入要解密的連線字串 (包含 encrypted: 前綴): ");
    var encryptedText = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(encryptedText))
    {
        Console.WriteLine("加密字串不能為空！");
        return;
    }

    // 移除 encrypted: 前綴
    if (encryptedText.StartsWith("encrypted:", StringComparison.OrdinalIgnoreCase))
    {
        encryptedText = encryptedText.Substring("encrypted:".Length);
    }

    Console.Write("請輸入加密金鑰 (Base64): ");
    var keyBase64 = Console.ReadLine();

    Console.Write("請輸入 IV (Base64): ");
    var ivBase64 = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(keyBase64) || string.IsNullOrWhiteSpace(ivBase64))
    {
        Console.WriteLine("金鑰和 IV 不能為空！");
        return;
    }

    try
    {
        // 建立臨時設定
        var configuration = new Dictionary<string, string?>
        {
            { "Encryption:Key", keyBase64 },
            { "Encryption:IV", ivBase64 }
        };

        var configBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(configuration);
        var config = configBuilder.Build();

        var encryptionService = new Aes256EncryptionService(config);
        var decrypted = encryptionService.Decrypt(encryptedText);

        Console.WriteLine("\n解密成功！");
        Console.WriteLine($"解密後的連線字串: {decrypted}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"解密失敗: {ex.Message}");
    }
}
