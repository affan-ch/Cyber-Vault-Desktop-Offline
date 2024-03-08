using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using Cyber_Vault.DB;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DL;

internal class SecureNoteDL
{
    private static readonly List<SecureNote> notes = new();

    // Add Secure Note
    public static void AddSecureNote(SecureNote note)
    {    
        notes.Add(note);
    }

    // Update Secure Note
    public static void UpdateSecureNote(SecureNote note)
    {
        var index = notes.FindIndex(n => n.Id == note.Id);
        notes[index] = note;
    }

    // Delete Secure Note
    public static void DeleteSecureNote(int id)
    {
        var note = notes.FirstOrDefault(n => n.Id == id);
        if (note != null)
        {
            notes.Remove(note);
        }
    }

    // Get All Secure Notes
    public static List<SecureNote> GetSecureNotes()
    {       
        return notes;
    }

    // Get Secure Note by Id
    public static SecureNote? GetSecureNoteById(int id)
    {          
        return notes.FirstOrDefault(n => n.Id == id);
    }

    // Get Secure Notes by Category
    public static List<SecureNote> GetSecureNotesByCategory(string category)
    {
        return notes.Where(n => n.Category == category).ToList();
    }

    // Get Secure Notes by Tag
    public static List<SecureNote> GetSecureNotesByTag(string tag)
    {
        return notes.Where(n => n.Tags!.Contains(tag)).ToList();
    }

    // Clear the Secure Note List
    public static void ClearSecureNotes()
    {          
        notes.Clear();
    }

    // Load Secure Notes Into List from SQLite Database
    public static void LoadSecureNotesFromDatabase()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
          
        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"SELECT * FROM SecureNote"
        };
        
        using var reader = command.ExecuteReader();
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
                var id = int.Parse(reader["Id"].ToString() ?? "0");
                var title = EncryptionHelper.Decrypt(reader["Title"].ToString() ?? "", username + password);
                var note = EncryptionHelper.Decrypt(reader["Note"].ToString() ?? "", username + password);
                var category = EncryptionHelper.Decrypt(reader["Category"].ToString() ?? "", username + password);
                var tags = EncryptionHelper.Decrypt(reader["Tags"].ToString() ?? "", username + password);
                var dateCreated = EncryptionHelper.Decrypt(reader["DateCreated"].ToString() ?? "", username + password);
                var dateModified = EncryptionHelper.Decrypt(reader["DateModified"].ToString() ?? "", username + password);
                
                var secureNote = new SecureNote(id, title, note, category, tags, dateCreated, dateModified);
                notes.Add(secureNote);
            }
        }
        finally
        {        
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }
       
        connection.Close();
    }

}
