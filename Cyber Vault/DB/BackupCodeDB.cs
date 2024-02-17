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
}

