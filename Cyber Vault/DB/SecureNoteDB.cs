using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;
internal class SecureNoteDB
{
    // Store Secure Notes in DB
    public static void StoreSecureNotes(SecureNote note)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"INSERT INTO SecureNote (Title, Category, Tag1, Tag2, Tag3, Tag4, Note, DateAdded, DateModified)
            VALUES (@Title, @Category, @Tag1, @Tag2, @Tag3, @Tag4, @Note, @DateAdded, @DateModified)"
        };

        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;

        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@Title", EncryptionHelper.Encrypt(note.Title ?? "", username + password));
            command.Parameters.AddWithValue("@Category", EncryptionHelper.Encrypt(note.Category ?? "", username + password));
            command.Parameters.AddWithValue("@Tag1", EncryptionHelper.Encrypt(note.Tag1 ?? "", username + password));
            command.Parameters.AddWithValue("@Tag2", EncryptionHelper.Encrypt(note.Tag2 ?? "", username + password));
            command.Parameters.AddWithValue("@Tag3", EncryptionHelper.Encrypt(note.Tag3 ?? "", username + password));
            command.Parameters.AddWithValue("@Tag4", EncryptionHelper.Encrypt(note.Tag4 ?? "", username + password));
            command.Parameters.AddWithValue("@Note", EncryptionHelper.Encrypt(note.Note ?? "", username + password));
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

    // Update Secure Note in DB
    public static void UpdateSecureNoteInDatabase(SecureNote note)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {

            CommandText = @"UPDATE SecureNote SET Title = @Title, Category = @Category, Tag1 = @Tag1, Tag2 = @Tag2, Tag3 = @Tag3, Tag4 = @Tag4, Note = @Note, DateModified = @DateModified WHERE Id = @Id"
        };

        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;

        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@Id", note.Id);
            command.Parameters.AddWithValue("@Title", EncryptionHelper.Encrypt(note.Title ?? "", username + password));
            command.Parameters.AddWithValue("@Category", EncryptionHelper.Encrypt(note.Category ?? "", username + password));
            command.Parameters.AddWithValue("@Tag1", EncryptionHelper.Encrypt(note.Tag1 ?? "", username + password));
            command.Parameters.AddWithValue("@Tag2", EncryptionHelper.Encrypt(note.Tag2 ?? "", username + password));
            command.Parameters.AddWithValue("@Tag3", EncryptionHelper.Encrypt(note.Tag3 ?? "", username + password));
            command.Parameters.AddWithValue("@Tag4", EncryptionHelper.Encrypt(note.Tag4 ?? "", username + password));
            command.Parameters.AddWithValue("@Note", EncryptionHelper.Encrypt(note.Note ?? "", username + password));
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

    // Delete Secure Note from DB
    public static void DeleteSecureNoteFromDatabase(int id)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"DELETE FROM SecureNote WHERE Id = @Id"
        };

        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();

        connection.Close();
    }

    // Get Max Id from SecureNote Table
    public static int GetMaxId()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT MAX(Id) FROM SecureNote"
        };

        var maxId = command.ExecuteScalar();
        connection.Close();

        return maxId is DBNull ? 0 : Convert.ToInt32(maxId);
    }

}
