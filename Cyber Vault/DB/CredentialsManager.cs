using System.Data.SQLite;
using System.Security;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;

internal class CredentialsManager
{
    private static SecureString? Username;
    private static SecureString? MasterPassword;


    // Store Key in Database
    public static void StoreInDatabase(string username, string password)
    {
        var encryptedKey = EncyptionHelper.Encrypt(username+password, username+password);
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
    
        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"INSERT INTO MasterKey (Key) VALUES (@Key)"
        };
    
        command.Parameters.AddWithValue("@Key", encryptedKey);
    
        command.ExecuteNonQuery();
    
        connection.Close();
    }

    // Check if Credentials are Correct
    public static bool MatchInDatabase(string username, string password)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
       
        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM MasterKey"
        };
       
        using var reader = command.ExecuteReader();
       
        if (reader.Read())
        {
            var encryptedKey = reader["Key"].ToString() ?? string.Empty;
            if (encryptedKey != string.Empty)
            {
                var decryptedKey = EncyptionHelper.Decrypt(encryptedKey, username+password);
                if(decryptedKey == username+password)
                {                
                    connection.Close();
                    return true;
                }
                
            }
        }
    
        connection.Close();
        return false;
    }

    // Check if Key is in Database
    public static bool CheckInDatabase()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
          
        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM MasterKey"
        };
          
        using var reader = command.ExecuteReader();
          
        if (reader.Read())
        {
            var encryptedKey = reader["Key"].ToString() ?? string.Empty;
            if (encryptedKey != string.Empty)
            {
                return true;
            }
        }
    
        connection.Close();
        return false;
    }


    // Get SecureString Master Username from Memory
    public static SecureString? GetUsernameFromMemory()
    {
        return Username;
    }

    // Get SecureString Master Password from Memory
    public static SecureString? GetPasswordFromMemory()
    {
        return MasterPassword;
    }


    // Delete Username from Memory
    public static void DeleteUsernameFromMemory()
    {
        Username = null;
    }

    // Delete Master Password from Memory
    public static void DeletePasswordFromMemory()
    {
        MasterPassword = null;
    }

    // Function to create a SecureString from a regular username
    public static void StoreUsernameInMemory(string username)
    {
        var secureString = new SecureString();

        // Convert the input string to a char array
        var chars = username.ToCharArray();

        // Populate the SecureString with the characters
        foreach (var c in chars)
        {
            secureString.AppendChar(c);
        }

        // Make the SecureString read-only and prevent it from being modified
        secureString.MakeReadOnly();

        Username = secureString;
    }

    // Function to create a SecureString from a regular password
    public static void StorePasswordInMemory(string password)
    {
        var secureString = new SecureString();

        // Convert the input string to a char array
        var chars = password.ToCharArray();

        // Populate the SecureString with the characters
        foreach (var c in chars)
        {
            secureString.AppendChar(c);
        }

        // Make the SecureString read-only and prevent it from being modified
        secureString.MakeReadOnly();

        MasterPassword = secureString;
    }

}
