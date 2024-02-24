using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using Cyber_Vault.DB;
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
        var index = backupCodes.FindIndex(b => b.AccountId == backupCode.AccountId && b.Code == backupCode.Code);
        if (index != -1)
        {
            backupCodes.RemoveAt(index);
            backupCodes.Add(backupCode);
        }
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

    // Delete Backup Codes by Account Id
    public static void DeleteBackupCodesByAccountId(int accountId)
    {
        backupCodes.RemoveAll(b => b.AccountId == accountId);
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
            var UsernamePtr = IntPtr.Zero;
            var PasswordPtr = IntPtr.Zero;
            try
            {
                UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
                PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
                var username = Marshal.PtrToStringUni(UsernamePtr);
                var password = Marshal.PtrToStringUni(PasswordPtr);

                var backupCode = new BackupCode(
                    Id: reader.GetInt32(0),
                    AccountId: reader.GetInt32(1),
                    Code: EncryptionHelper.Decrypt(reader.GetString(2), username+password),
                    IsUsed: reader.GetInt32(3),
                    DateAdded: reader.GetString(4),
                    DateModified: reader.GetString(5)
                );

                backupCodes.Add(backupCode);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
                Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
            }

        }
    
        connection.Close();
    }



}
