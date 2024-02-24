using System.Data.SQLite;
using System.Runtime.InteropServices;
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

            var UsernamePtr = IntPtr.Zero;
            var PasswordPtr = IntPtr.Zero;
            try
            {
                UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
                PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
                var username = Marshal.PtrToStringUni(UsernamePtr);
                var password = Marshal.PtrToStringUni(PasswordPtr);

                command.Parameters.AddWithValue("@AccountId", backupCode.AccountId);
                command.Parameters.AddWithValue("@Code", EncryptionHelper.Encrypt(backupCode.Code ?? "", username + password));

                command.ExecuteNonQuery();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
                Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
            }

        }

        connection.Close();
    }

    // DeleteBackupCodesByAccountId
    public static void DeleteBackupCodesByAccountId(int accountId)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
    
        using var command = new SQLiteCommand(connection)
        {     
            CommandText = "DELETE FROM BackupCode WHERE AccountId = @AccountId"
        };
    
        command.Parameters.AddWithValue("@AccountId", accountId);
        command.ExecuteNonQuery();
    
        connection.Close();
    }

    public static void UpdateBackupCodeStatus(int accountId, string backupCode, int isUsed)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM BackupCode WHERE AccountId = @AccountId"
        };

        command.Parameters.AddWithValue("@AccountId", accountId);
        using var reader = command.ExecuteReader();

        var encryptedBackupCode = "";
        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            while (reader.Read())
            {
                var decryptedBackupCode = EncryptionHelper.Decrypt(reader.GetString(2), username + password);

                if (decryptedBackupCode == backupCode)
                {
                    encryptedBackupCode = reader.GetString(2);
                    break;
                }
            }
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }

        using var updateCommand = new SQLiteCommand(connection)
        {
            CommandText = "UPDATE BackupCode SET IsUsed = @IsUsed WHERE AccountId = @AccountId AND Code = @Code"
        };

        updateCommand.Parameters.AddWithValue("@AccountId", accountId);
        updateCommand.Parameters.AddWithValue("@Code", encryptedBackupCode);
        updateCommand.Parameters.AddWithValue("@IsUsed", isUsed);

        updateCommand.ExecuteNonQuery();

        connection.Close();

    }
}

