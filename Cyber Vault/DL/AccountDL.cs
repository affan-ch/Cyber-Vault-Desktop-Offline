using System.Data.SQLite;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;
using Cyber_Vault.DB;
using System.Runtime.InteropServices;

namespace Cyber_Vault.DL;

internal class AccountDL
{
    private static readonly List<Account> accounts = new();

    // Add Account
    public static void AddAccount(Account account)
    {
        accounts.Add(account);
    }

    // Update Account
    public static void UpdateAccount(Account account)
    {
        var index = accounts.FindIndex(a => a.Id == account.Id);
        accounts[index] = account;
    }

    // Delete Account
    public static void DeleteAccount(int id)
    {
        var account = accounts.FirstOrDefault(a => a.Id == id);
        if (account != null)
        {
            accounts.Remove(account);
        }
    }

    // Get All Accounts
    public static List<Account> GetAccounts()
    {
        return accounts;
    }

    // Get Account by Id
    public static Account? GetAccountById(int id)
    {
        return accounts.FirstOrDefault(a => a.Id == id);
    }

    // Clear the Account List
    public static void ClearAccounts()
    {
        accounts.Clear();
    }

    // Get All Accounts by Type
    public static List<Account> GetAccountsByType(string type)
    {
        return accounts.Where(a => a.Type == type).ToList();
    }

    // Load Accounts Into List from SQLite Database
    public static void LoadAccountsFromDatabase()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();
    
        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM Account"
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
                var account = new Account
                (
                    Id: int.Parse(reader["Id"].ToString() ?? "0"),
                    Type: EncryptionHelper.Decrypt(reader["Type"].ToString() ?? "", username + password),
                    Title: EncryptionHelper.Decrypt(reader["Title"].ToString() ?? "", username + password),
                    Domain: EncryptionHelper.Decrypt(reader["Domain"].ToString() ?? "", username + password),
                    Name: EncryptionHelper.Decrypt(reader["Name"].ToString() ?? "", username + password),
                    Email: EncryptionHelper.Decrypt(reader["Email"].ToString() ?? "", username + password),
                    Username: EncryptionHelper.Decrypt(reader["Username"].ToString() ?? "", username + password),
                    PhoneNumber: EncryptionHelper.Decrypt(reader["PhoneNumber"].ToString() ?? "", username + password),
                    Password: EncryptionHelper.Decrypt(reader["Password"].ToString() ?? "", username + password),
                    Pin: EncryptionHelper.Decrypt(reader["Pin"].ToString() ?? "", username + password),
                    DateOfBirth: EncryptionHelper.Decrypt(reader["DateOfBirth"].ToString() ?? "", username + password),
                    RecoveryEmail: EncryptionHelper.Decrypt(reader["RecoveryEmail"].ToString() ?? "", username + password),
                    RecoveryPhoneNumber: EncryptionHelper.Decrypt(reader["RecoveryPhoneNumber"].ToString() ?? "", username + password),
                    QrCode: EncryptionHelper.Decrypt(reader["QrCode"].ToString() ?? "", username + password),
                    Notes: EncryptionHelper.Decrypt(reader["Notes"].ToString() ?? "", username + password),
                    DateAdded: reader["DateAdded"].ToString() ?? "",
                    DateModified: reader["DateModified"].ToString() ?? ""
                );
                accounts.Add(account);
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
