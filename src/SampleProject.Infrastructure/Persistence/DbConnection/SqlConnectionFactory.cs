using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SampleProject.Infrastructure.Encryption;

namespace SampleProject.Infrastructure.Persistence.DbConnection;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration, IEncryptionService? encryptionService = null)
    {
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string is not configured.");

        // 如果連線字串以 "encrypted:" 開頭，則進行解密
        if (rawConnectionString.StartsWith("encrypted:", StringComparison.OrdinalIgnoreCase))
        {
            if (encryptionService == null)
            {
                throw new InvalidOperationException(
                    "Encrypted connection string detected but IEncryptionService is not registered.");
            }

            var encryptedValue = rawConnectionString.Substring("encrypted:".Length);
            _connectionString = encryptionService.Decrypt(encryptedValue);
        }
        else
        {
            _connectionString = rawConnectionString;
        }
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
