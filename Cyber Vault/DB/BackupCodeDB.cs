using System.Data.SQLite;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;
internal class BackupCodeDB
{
    // Store Backup Codes to SQLite Database
    public static void StoreBackupCodes(List<BackupCode> backupCodes)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection);

        foreach (var backupCode in backupCodes)
        {
            command.CommandText = @"INSERT INTO BackupCode (AccountId, Code) VALUES (@AccountId, @Code)";

            command.Parameters.AddWithValue("@AccountId", backupCode.AccountId);
            command.Parameters.AddWithValue("@Code", backupCode.Code);

            command.ExecuteNonQuery();
        }

        connection.Close();
    }
}

