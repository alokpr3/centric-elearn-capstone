using System.Reflection;
using DbUp;

namespace Harbourline.Api.Data;

public static class DatabaseMigrator
{
    public static void Run(string connectionString)
    {
        var upgrader = DeployChanges.To
            .MySqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), name => name.EndsWith(".sql"))
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new InvalidOperationException("Database migration failed", result.Error);
    }
}
