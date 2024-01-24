using System.Data.SQLite;
using Cyber_Vault.BL;
using Cyber_Vault.Utils;

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
    public static void DeleteAccount(Account account)
    {
        accounts.Remove(account);
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
                Type: reader["Type"].ToString() ?? "",
                Title: reader["Title"].ToString() ?? "",
                Domain: reader["Domain"].ToString() ?? "",
                Name: reader["Name"].ToString() ?? "",
                Email: reader["Email"].ToString() ?? "",
                Username: reader["Username"].ToString() ?? "",
                PhoneNumber: reader["PhoneNumber"].ToString() ?? "",
                Password: reader["Password"].ToString() ?? "",
                Pin: reader["Pin"].ToString() ?? "",
                DateOfBirth: reader["DateOfBirth"].ToString() ?? "",
                RecoveryEmail: reader["RecoveryEmail"].ToString() ?? "",
                RecoveryPhoneNumber: reader["RecoveryPhoneNumber"].ToString() ?? "",
                QrCode: reader["QrCode"].ToString() ?? "",
                Secret: reader["Secret"].ToString() ?? "",
                Notes: reader["Notes"].ToString() ?? "",
                DateAdded: reader["DateAdded"].ToString() ?? "",
                DateModified: reader["DateModified"].ToString() ?? ""
            );
        
            accounts.Add(account);
        }

        connection.Close();
    }

}
