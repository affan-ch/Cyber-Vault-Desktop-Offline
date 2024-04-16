using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using Cyber_Vault.Utils;

namespace Cyber_Vault.DB;

internal class CreditCardDB
{
    // Add a new credit card to the database
    public static void StoreCreditCard(CreditCard creditCard)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"INSERT INTO CreditCard (CardHolderName, CardNumber, ExpiryMonth, ExpiryYear, CVV, Pin, CardIssuer, CardType, BillingAddress, City, State, ZipCode, Country, Notes)
            VALUES (@CardHolderName, @CardNumber, @ExpiryMonth, @ExpiryYear, @CVV, @Pin, @CardIssuer, @CardType, @BillingAddress, @City, @State, @ZipCode, @Country, @Notes)"
        };


        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@CardHolderName", EncryptionHelper.Encrypt(creditCard.CardHolderName ?? "", username + password));
            command.Parameters.AddWithValue("@CardNumber", EncryptionHelper.Encrypt(creditCard.CardNumber ?? "", username + password));
            command.Parameters.AddWithValue("@ExpiryMonth", EncryptionHelper.Encrypt(creditCard.ExpiryMonth ?? "", username + password));
            command.Parameters.AddWithValue("@ExpiryYear", EncryptionHelper.Encrypt(creditCard.ExpiryYear ?? "", username + password));
            command.Parameters.AddWithValue("@CVV", EncryptionHelper.Encrypt(creditCard.CVV ?? "", username + password));
            command.Parameters.AddWithValue("@Pin", EncryptionHelper.Encrypt(creditCard.Pin ?? "", username + password));
            command.Parameters.AddWithValue("@CardIssuer", EncryptionHelper.Encrypt(creditCard.CardIssuer ?? "", username + password));
            command.Parameters.AddWithValue("@CardType", EncryptionHelper.Encrypt(creditCard.CardType ?? "", username + password));
            command.Parameters.AddWithValue("@BillingAddress", EncryptionHelper.Encrypt(creditCard.BillingAddress ?? "", username + password));
            command.Parameters.AddWithValue("@City", EncryptionHelper.Encrypt(creditCard.City ?? "", username + password));
            command.Parameters.AddWithValue("@State", EncryptionHelper.Encrypt(creditCard.State ?? "", username + password));
            command.Parameters.AddWithValue("@ZipCode", EncryptionHelper.Encrypt(creditCard.ZipCode ?? "", username + password));
            command.Parameters.AddWithValue("@Country", EncryptionHelper.Encrypt(creditCard.Country ?? "", username + password));
            command.Parameters.AddWithValue("@Notes", EncryptionHelper.Encrypt(creditCard.Notes ?? "", username + password));

            command.ExecuteNonQuery();
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }

        connection.Close();
    }

    // Delete Credit Card
    public static void DeleteCreditCard(int id)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
    
        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"DELETE FROM CreditCard WHERE Id = @Id"
        };
    
        command.Parameters.AddWithValue("@Id", id);
    
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
            CommandText = @"SELECT MAX(Id) FROM CreditCard"
        };

        using var reader = command.ExecuteReader();
        reader.Read();

        try
        {
            var maxId = int.Parse(reader[0].ToString() ?? "0");
            connection.Close();
            return maxId;
        }
        catch
        {
            connection.Close();
            return 0;
        }
    }
    // Update Credit Card
    public static void UpdateCreditCard(CreditCard creditCard)
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection)
        {
            CommandText = @"UPDATE CreditCard SET CardHolderName = @CardHolderName, CardNumber = @CardNumber, ExpiryMonth = @ExpiryMonth, ExpiryYear = @ExpiryYear, CVV = @CVV, Pin = @Pin, CardIssuer = @CardIssuer, CardType = @CardType, BillingAddress = @BillingAddress, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country, Notes = @Notes WHERE Id = @Id"
        };

        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            command.Parameters.AddWithValue("@Id", creditCard.Id);
            command.Parameters.AddWithValue("@CardHolderName", EncryptionHelper.Encrypt(creditCard.CardHolderName ?? "", username + password));
            command.Parameters.AddWithValue("@CardNumber", EncryptionHelper.Encrypt(creditCard.CardNumber ?? "", username + password));
            command.Parameters.AddWithValue("@ExpiryMonth", EncryptionHelper.Encrypt(creditCard.ExpiryMonth ?? "", username + password));
            command.Parameters.AddWithValue("@ExpiryYear", EncryptionHelper.Encrypt(creditCard.ExpiryYear ?? "", username + password));
            command.Parameters.AddWithValue("@CVV", EncryptionHelper.Encrypt(creditCard.CVV ?? "", username + password));
            command.Parameters.AddWithValue("@Pin", EncryptionHelper.Encrypt(creditCard.Pin ?? "", username + password));
            command.Parameters.AddWithValue("@CardIssuer", EncryptionHelper.Encrypt(creditCard.CardIssuer ?? "", username + password));
            command.Parameters.AddWithValue("@CardType", EncryptionHelper.Encrypt(creditCard.CardType ?? "", username + password));
            command.Parameters.AddWithValue("@BillingAddress", EncryptionHelper.Encrypt(creditCard.BillingAddress ?? "", username + password));
            command.Parameters.AddWithValue("@City", EncryptionHelper.Encrypt(creditCard.City ?? "", username + password));
            command.Parameters.AddWithValue("@State", EncryptionHelper.Encrypt(creditCard.State ?? "", username + password));
            command.Parameters.AddWithValue("@ZipCode", EncryptionHelper.Encrypt(creditCard.ZipCode ?? "", username + password));
            command.Parameters.AddWithValue("@Country", EncryptionHelper.Encrypt(creditCard.Country ?? "", username + password));
            command.Parameters.AddWithValue("@Notes", EncryptionHelper.Encrypt(creditCard.Notes ?? "", username + password));

        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
            Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
        }

        command.ExecuteNonQuery();

        connection.Close();
    }

}
