
using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;
using Microsoft.UI.Xaml;

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


        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@Type", EncryptionHelper.Encrypt(account.Type ?? "", username + password));
            command.Parameters.AddWithValue("@Title", EncryptionHelper.Encrypt(account.Title ?? "", username + password));
            command.Parameters.AddWithValue("@Domain", EncryptionHelper.Encrypt(account.Domain ?? "", username + password));
            command.Parameters.AddWithValue("@Name", EncryptionHelper.Encrypt(account.Name ?? "", username + password));
            command.Parameters.AddWithValue("@Email", EncryptionHelper.Encrypt(account.Email ?? "", username + password));
            command.Parameters.AddWithValue("@Username", EncryptionHelper.Encrypt(account.Username ?? "", username + password));
            command.Parameters.AddWithValue("@PhoneNumber", EncryptionHelper.Encrypt(account.PhoneNumber ?? "", username + password));
            command.Parameters.AddWithValue("@Password", EncryptionHelper.Encrypt(account.Password ?? "", username + password));
            command.Parameters.AddWithValue("@Pin", EncryptionHelper.Encrypt(account.Pin ?? "", username + password));
            command.Parameters.AddWithValue("@DateOfBirth", EncryptionHelper.Encrypt(account.DateOfBirth ?? "", username + password));
            command.Parameters.AddWithValue("@RecoveryEmail", EncryptionHelper.Encrypt(account.RecoveryEmail ?? "", username + password));
            command.Parameters.AddWithValue("@RecoveryPhoneNumber", EncryptionHelper.Encrypt(account.RecoveryPhoneNumber ?? "", username + password));
            command.Parameters.AddWithValue("@QrCode", EncryptionHelper.Encrypt(account.QrCode ?? "", username + password));
            command.Parameters.AddWithValue("@Secret", EncryptionHelper.Encrypt(account.Secret ?? "", username + password));
            command.Parameters.AddWithValue("@Notes", EncryptionHelper.Encrypt(account.Notes ?? "", username + password));

            command.ExecuteNonQuery();

        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }


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

        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@Id", account.Id);
            command.Parameters.AddWithValue("@Type", EncryptionHelper.Encrypt(account.Type ?? "", username + password));
            command.Parameters.AddWithValue("@Title", EncryptionHelper.Encrypt(account.Title ?? "", username + password));
            command.Parameters.AddWithValue("@Domain", EncryptionHelper.Encrypt(account.Domain ?? "", username + password));
            command.Parameters.AddWithValue("@Name", EncryptionHelper.Encrypt(account.Name ?? "", username + password));
            command.Parameters.AddWithValue("@Email", EncryptionHelper.Encrypt(account.Email ?? "", username + password));
            command.Parameters.AddWithValue("@Username", EncryptionHelper.Encrypt(account.Username ?? "", username + password));
            command.Parameters.AddWithValue("@PhoneNumber", EncryptionHelper.Encrypt(account.PhoneNumber ?? "", username + password));
            command.Parameters.AddWithValue("@Password", EncryptionHelper.Encrypt(account.Password ?? "", username + password));
            command.Parameters.AddWithValue("@Pin", EncryptionHelper.Encrypt(account.Pin ?? "", username + password));
            command.Parameters.AddWithValue("@DateOfBirth", EncryptionHelper.Encrypt(account.DateOfBirth ?? "", username + password));
            command.Parameters.AddWithValue("@RecoveryEmail", EncryptionHelper.Encrypt(account.RecoveryEmail ?? "", username + password));
            command.Parameters.AddWithValue("@RecoveryPhoneNumber", EncryptionHelper.Encrypt(account.RecoveryPhoneNumber ?? "", username + password));
            command.Parameters.AddWithValue("@QrCode", EncryptionHelper.Encrypt(account.QrCode ?? "", username + password));
            command.Parameters.AddWithValue("@Secret", EncryptionHelper.Encrypt(account.Secret ?? "", username + password));
            command.Parameters.AddWithValue("@Notes", EncryptionHelper.Encrypt(account.Notes ?? "", username + password));

        }
        finally
        {        
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }
    
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


    // Get Max Id
    public static int GetMaxId()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
       
        using var command = new SQLiteCommand(connection)
        {  
            CommandText = @"SELECT MAX(Id) FROM Account"
        };
       
        using var reader = command.ExecuteReader();
       
        reader.Read();
        try
        {
            var maxId = int.Parse(reader[0].ToString() ?? "0");
            return maxId;
        }
        catch
        {
            connection.Close();
            return 0;
        }       
    }


}
