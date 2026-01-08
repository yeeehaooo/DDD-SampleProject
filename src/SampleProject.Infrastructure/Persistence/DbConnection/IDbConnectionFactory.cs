using System.Data;

namespace SampleProject.Infrastructure.Persistence.DbConnection;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
