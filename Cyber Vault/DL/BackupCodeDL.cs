using System.Data.SQLite;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DL;

internal class BackupCodeDL
{
    private static readonly List<BackupCode> backupCodes = new();

    // Add Backup Code
    public static void AddBackupCode(BackupCode backupCode)
    {
        backupCodes.Add(backupCode);
    }


    // Update Backup Code
    public static void UpdateBackupCode(BackupCode backupCode)
    {
        var index = backupCodes.FindIndex(b => b.Id == backupCode.Id);
        backupCodes[index] = backupCode;
    }


    // Delete Backup Code
    public static void DeleteBackupCode(BackupCode backupCode)
    {
        backupCodes.Remove(backupCode);
    }

    // Get All Backup Codes
    public static List<BackupCode> GetBackupCodes()
    {
        return backupCodes;
    }

    // Get All Backup Codes by Account Id
    public static List<BackupCode> GetBackupCodesByAccountId(int accountId)
    {
        return backupCodes.Where(b => b.AccountId == accountId).ToList();
    }



    // Load Backup Codes Into List from SQLite Database
    public static void LoadBackupCodesFromDatabase()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
       
        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM BackupCode"
        };
       
        using var reader = command.ExecuteReader();
       
        while (reader.Read())
        {
            var backupCode = new BackupCode(
                Id: reader.GetInt32(0),
                AccountId: reader.GetInt32(1),
                Code: reader.GetString(2),
                IsUsed: reader.GetInt32(3),
                DateAdded: reader.GetString(4),
                DateUsed: reader.GetString(5),
                DateModified: reader.GetString(6)
            );
            
            backupCodes.Add(backupCode);
        }
    
        connection.Close();
    }



}
