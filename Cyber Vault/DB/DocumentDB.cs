using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
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
            CommandText = @"INSERT INTO Document (Type, Title, DateAdded, DateModified)
            VALUES (@Type, @Title, @DateAdded, @DateModified)"
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
            command.Parameters.AddWithValue("@DateAdded", EncryptionHelper.Encrypt(DateTime.Now.ToString(), username + password));
            command.Parameters.AddWithValue("@DateModified", EncryptionHelper.Encrypt(DateTime.Now.ToString(), username + password));

            command.ExecuteNonQuery();
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }
        connection.Close();
    }

    // Store Document File in DB
    public static void StoreDocumentFile(DocumentFile documentFile)
    {

        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {

            CommandText = @"INSERT INTO DocumentFile (DocumentId, FileName, FileType, FileContent)
            VALUES (@DocumentId, @FileName, @FileType, @FileContent)"
        };

        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {

            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@DocumentId", documentFile.DocumentId);
            command.Parameters.AddWithValue("@FileName", EncryptionHelper.Encrypt(documentFile.FileName ?? "", username + password));
            command.Parameters.AddWithValue("@FileType", EncryptionHelper.Encrypt(documentFile.FileType ?? "", username + password));
            command.Parameters.AddWithValue("@FileContent", EncryptionHelper.DocumentEncrypt(documentFile.FileContent ?? Array.Empty<byte>(), username + password));

            command.ExecuteNonQuery();
        }
        finally
        {

            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }
        connection.Close();
    }

    // Get Max Document Id
    public static int GetMaxDocumentId()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT MAX(Id) FROM Document"
        };

        var maxId = command.ExecuteScalar();
        connection.Close();

        return maxId is null ? 0 : Convert.ToInt32(maxId);
    }

}
