using System.Data.SQLite;
using System.Diagnostics;

namespace Cyber_Vault.Utils;

internal class DatabaseHelper
{
    private static readonly string DatabaseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Cyber Vault");

    private static readonly string DatabaseFilePath = Path.Combine(DatabaseFolderPath, "CyberVault.db");

    public static readonly string ConnectionString = $"Data Source={DatabaseFilePath};Version=3;";

    private static readonly string CreateAccountTableQuery = @"CREATE TABLE IF NOT EXISTS [Account] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Type TEXT NOT NULL,
        Title TEXT NOT NULL,
        Domain TEXT NOT NULL,
        Name TEXT,
        Email TEXT,
        Username TEXT,
        PhoneNumber TEXT,
        Password TEXT,
        Pin TEXT,
        DateOfBirth TEXT,
        RecoveryEmail TEXT,
        RecoveryPhoneNumber TEXT,
        QrCode TEXT,
        Secret TEXT,
        Notes TEXT,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        DateModified TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";

    private static readonly string CreatePasswordHistoryTableQuery = @"CREATE TABLE IF NOT EXISTS [PasswordHistory] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        AccountId INTEGER NOT NULL,
        Password TEXT NOT NULL,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";

    private static readonly string CreateBackupCodeTableQuery = @"CREATE TABLE IF NOT EXISTS [BackupCode] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        AccountId INTEGER NOT NULL,
        Code TEXT NOT NULL,
        IsUsed INTEGER NOT NULL,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        DateModified TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";

    private static readonly string UpdateAccountTriggerQuery = @"CREATE TRIGGER IF NOT EXISTS [Account_UpdateTrigger]
        AFTER UPDATE ON Account
        BEGIN
            UPDATE Account SET DateModified = DATETIME('now') WHERE Id = OLD.Id;
        END;";

    private static readonly string UpdateBackupCodeTriggerQuery = @"CREATE TRIGGER IF NOT EXISTS [BackupCode_UpdateTrigger]
        AFTER UPDATE ON BackupCode
        BEGIN
            UPDATE BackupCode SET DateModified = DATETIME('now') WHERE Id = OLD.Id;
        END;";

    private static readonly string CreateMasterKeyTableQuery = @"CREATE TABLE IF NOT EXISTS [MasterKey] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Key TEXT NOT NULL,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";

    public static void CreateDatabase()
    {
        if (!Directory.Exists(DatabaseFolderPath))
        {
            var directoryInfo = Directory.CreateDirectory(DatabaseFolderPath);
            directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
            Debug.WriteLine("Settings folder created");
        }

        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();
        
        using var CreateAccountTableCommand = new SQLiteCommand(CreateAccountTableQuery, connection);
        CreateAccountTableCommand.ExecuteNonQuery();

        using var UpdateAccountTriggerCommand = new SQLiteCommand(UpdateAccountTriggerQuery, connection);
        UpdateAccountTriggerCommand.ExecuteNonQuery();

        using var CreatePasswordHistoryTableCommand = new SQLiteCommand(CreatePasswordHistoryTableQuery, connection);
        CreatePasswordHistoryTableCommand.ExecuteNonQuery();

        using var CreateBackupCodeTableCommand = new SQLiteCommand(CreateBackupCodeTableQuery, connection);
        CreateBackupCodeTableCommand.ExecuteNonQuery();

        using var UpdateBackupCodeTriggerCommand = new SQLiteCommand(UpdateBackupCodeTriggerQuery, connection);
        UpdateBackupCodeTriggerCommand.ExecuteNonQuery();

        using var CreateMasterKeyTableCommand = new SQLiteCommand(CreateMasterKeyTableQuery, connection);
        CreateMasterKeyTableCommand.ExecuteNonQuery();

        connection.Close();
    }



}
