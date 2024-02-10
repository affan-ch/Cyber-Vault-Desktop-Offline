using System.Data.SQLite;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;
using Cyber_Vault.DB;

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
    
        while (reader.Read())
        {
            var account = new Account
            (             
                Id: int.Parse(reader["Id"].ToString() ?? "0"),
                Type: EncyptionHelper.Decrypt(reader["Type"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Title:  EncyptionHelper.Decrypt(reader["Title"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Domain: EncyptionHelper.Decrypt(reader["Domain"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Name: EncyptionHelper.Decrypt(reader["Name"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Email: EncyptionHelper.Decrypt(reader["Email"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Username: EncyptionHelper.Decrypt(reader["Username"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                PhoneNumber: EncyptionHelper.Decrypt(reader["PhoneNumber"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Password: EncyptionHelper.Decrypt(reader["Password"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Pin: EncyptionHelper.Decrypt(reader["Pin"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                DateOfBirth: EncyptionHelper.Decrypt(reader["DateOfBirth"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                RecoveryEmail: EncyptionHelper.Decrypt(reader["RecoveryEmail"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                RecoveryPhoneNumber: EncyptionHelper.Decrypt(reader["RecoveryPhoneNumber"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                QrCode: EncyptionHelper.Decrypt(reader["QrCode"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Secret: EncyptionHelper.Decrypt(reader["Secret"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                Notes: EncyptionHelper.Decrypt(reader["Notes"].ToString() ?? "", CredentialsManager.GetUsernameFromMemory()!.ToString() + CredentialsManager.GetPasswordFromMemory()!.ToString()),
                DateAdded: reader["DateAdded"].ToString() ?? "",
                DateModified: reader["DateModified"].ToString() ?? ""
            );
        
            accounts.Add(account);
        }

        connection.Close();
    }

}
