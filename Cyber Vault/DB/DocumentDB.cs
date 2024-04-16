using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;
internal class DocumentDB
{
    // Add Document Record in DB
    public static void StoreDocument(Document document)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"INSERT INTO Document (Type, Title, Document)
            VALUES (@Type, @Title, @Document)"
        };


        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@Type", EncryptionHelper.Encrypt(document.Type ?? "", username + password));
            command.Parameters.AddWithValue("@Title", EncryptionHelper.Encrypt(document.Title ?? "", username + password));
            command.Parameters.AddWithValue("@Document", EncryptionHelper.DocumentEncrypt(data: document.Document1, pin: username + password));
            command.Parameters.AddWithValue("@Document2", EncryptionHelper.DocumentEncrypt(data: document.Document2, pin: username + password));
            
            command.ExecuteNonQuery();

        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }


        connection.Close();
    }

}
