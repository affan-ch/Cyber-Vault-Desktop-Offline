using System.Data.SQLite;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;

internal class MasterKey
{
    private static string? Key;

    // Store Key in Database
    public static void StoreInDatabase(string key)
    {
        var encryptedKey = EncyptionHelper.Encrypt(key, key);
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

    // Get Key from Database
    public static bool IsCorrect(string key)
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
                var decryptedKey = EncyptionHelper.Decrypt(encryptedKey, key);
                if(decryptedKey == key)
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
    public static bool IsInDatabase()
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


    // Get Key from Memory
    public static string GetFromMemory()
    {
        return Key ?? "";
    }

    // Store Key in Memory
    public static void StoreInMemory(string key)
    {
        Key = key;
    }

    // Delete Key from Memory
    public static void DeleteFromMemory()
    {
        Key = null;
    }
}
