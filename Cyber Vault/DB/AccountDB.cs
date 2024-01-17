
using System.Data.SQLite;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;

internal class AccountDB
{
    // Add Account Record in DB
    public static void StoreAccount(Account account)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"INSERT INTO Account (Type, Title, Domain, Name, Email, Username, PhoneNumber, Password, Pin, DateOfBirth, RecoveryEmail, RecoveryPhoneNumber, QrCode, Secret, Notes)
            VALUES (@Type, @Title, @Domain, @Name, @Email, @Username, @PhoneNumber, @Password, @Pin, @DateOfBirth, @RecoveryEmail, @RecoveryPhoneNumber, @QrCode, @Secret, @Notes)"
        };

        command.Parameters.AddWithValue("@Type", account.Type);
        command.Parameters.AddWithValue("@Title", account.Title);
        command.Parameters.AddWithValue("@Domain", account.Domain);
        command.Parameters.AddWithValue("@Name", account.Name);
        command.Parameters.AddWithValue("@Email", account.Email);
        command.Parameters.AddWithValue("@Username", account.Username);
        command.Parameters.AddWithValue("@PhoneNumber", account.PhoneNumber);
        command.Parameters.AddWithValue("@Password", account.Password);
        command.Parameters.AddWithValue("@Pin", account.Pin);
        command.Parameters.AddWithValue("@DateOfBirth", account.DateOfBirth);
        command.Parameters.AddWithValue("@RecoveryEmail", account.RecoveryEmail);
        command.Parameters.AddWithValue("@RecoveryPhoneNumber", account.RecoveryPhoneNumber);
        command.Parameters.AddWithValue("@QrCode", account.QrCode);
        command.Parameters.AddWithValue("@Secret", account.Secret);
        command.Parameters.AddWithValue("@Notes", account.Notes);

        command.ExecuteNonQuery();

        connection.Close();
    }


    // Update Account Record in DB
    public static void UpdateAccount(Account account)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
    
        using var command = new SQLiteCommand(connection)
        {
           CommandText = @"UPDATE Account SET Type = @Type, Title = @Title, Domain = @Domain, Name = @Name, Email = @Email, Username = @Username, PhoneNumber = @PhoneNumber, Password = @Password, Pin = @Pin, DateOfBirth = @DateOfBirth, RecoveryEmail = @RecoveryEmail, RecoveryPhoneNumber = @RecoveryPhoneNumber, QrCode = @QrCode, Secret = @Secret, Notes = @Notes WHERE Id = @Id"
        };
    
        command.Parameters.AddWithValue("@Id", account.Id);
        command.Parameters.AddWithValue("@Type", account.Type);
        command.Parameters.AddWithValue("@Title", account.Title);
        command.Parameters.AddWithValue("@Domain", account.Domain);
        command.Parameters.AddWithValue("@Name", account.Name);
        command.Parameters.AddWithValue("@Email", account.Email);
        command.Parameters.AddWithValue("@Username", account.Username);
        command.Parameters.AddWithValue("@PhoneNumber", account.PhoneNumber);
        command.Parameters.AddWithValue("@Password", account.Password);
        command.Parameters.AddWithValue("@Pin", account.Pin);
        command.Parameters.AddWithValue("@DateOfBirth", account.DateOfBirth);
        command.Parameters.AddWithValue("@RecoveryEmail", account.RecoveryEmail);
        command.Parameters.AddWithValue("@RecoveryPhoneNumber", account.RecoveryPhoneNumber);
        command.Parameters.AddWithValue("@QrCode", account.QrCode);
        command.Parameters.AddWithValue("@Secret", account.Secret);
        command.Parameters.AddWithValue("@Notes", account.Notes);
    
        command.ExecuteNonQuery();

        connection.Close();
    }


    // Delete Account Record in DB
    public static void DeleteAccount(int Id)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
       
        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"DELETE FROM Account WHERE Id = @Id"
        };
       
        command.Parameters.AddWithValue("@Id", Id);
       
        command.ExecuteNonQuery();
    
        connection.Close();
    }

}
