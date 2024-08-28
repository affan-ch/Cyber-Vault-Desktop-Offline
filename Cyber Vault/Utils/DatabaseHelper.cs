using System.Data.SQLite;
using System.Diagnostics;

namespace Cyber_Vault.Utils;

internal class DatabaseHelper
{
    private static readonly string AppDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cyber Vault");

    public static readonly string ThumbsFolderPath = Path.Combine(AppDataFolderPath, "Thumbs");

    private static readonly string DatabaseFilePath = Path.Combine(AppDataFolderPath, "CyberVault.db");

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
        TfaPhoneNumber TEXT,
        IsFavourite INTEGER DEFAULT 0,
        IsDeleted INTEGER DEFAULT 0,
        Tag1 TEXT,
        Tag2 TEXT,
        Tag3 TEXT,
        Tag4 TEXT,
        FolderId INTEGER,
        Notes TEXT,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        DateModified TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";


    private static readonly string CreateCustomFieldTableQuery = @"CREATE TABLE IF NOT EXISTS [CustomField] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        RecordTable INTEGER NOT NULL,
        RecordId INTEGER NOT NULL,
        FieldName TEXT NOT NULL,
        FieldValue TEXT NOT NULL
    );";

    private static readonly string CreateDocumentTableQuery = @"CREATE TABLE IF NOT EXISTS [Document] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Type TEXT NOT NULL,
        Title TEXT NOT NULL,
        DateAdded TEXT NOT NULL,
        DateModified TEXT NOT NULL
    );";

    // folder table
    private static readonly string CreateFolderTableQuery = @"CREATE TABLE IF NOT EXISTS [Folder] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Name TEXT NOT NULL,
        ParentId INTEGER,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        DateModified TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";

    private static readonly string CreateDocumentFilesTableQuery = @"CREATE TABLE IF NOT EXISTS [DocumentFile] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        DocumentId INTEGER NOT NULL,
        FileName TEXT NOT NULL,
        FileType TEXT NOT NULL,
        FileContent BLOB NOT NULL
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
        IsUsed INTEGER DEFAULT 0,
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

    private static readonly string CreateCreditCardTableQuery = @"CREATE TABLE IF NOT EXISTS [CreditCard] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        CardHolderName TEXT,
        CardNumber TEXT NOT NULL,
        ExpiryMonth TEXT NOT NULL,
        ExpiryYear TEXT NOT NULL,
        CVV TEXT NOT NULL,
        Pin TEXT,
        CardIssuer TEXT,
        CardType TEXT,
        BillingAddress TEXT,
        City TEXT,
        State TEXT,
        ZipCode TEXT,
        Country TEXT,
        Notes TEXT,
        DateAdded TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        DateModified TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );";

    private static readonly string CreateSecureNoteTableQuery = @"CREATE TABLE IF NOT EXISTS [SecureNote] (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Title TEXT NOT NULL,
        Category TEXT NOT NULL,
        Tag1 TEXT,
        Tag2 TEXT,
        Tag3 TEXT,
        Tag4 TEXT,
        Note TEXT,
        DateAdded TEXT,
        DateModified TEXT
    );";

    public static void CreateDatabase()
    {
        if (!Directory.Exists(AppDataFolderPath))
        {
            var directoryInfo = Directory.CreateDirectory(AppDataFolderPath);
            directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
            Debug.WriteLine("App Data folder created");
        }

        if (!Directory.Exists(ThumbsFolderPath))
        {
            Directory.CreateDirectory(ThumbsFolderPath);
            Debug.WriteLine("Thumbs folder created");
        }

        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var CreateAccountTableCommand = new SQLiteCommand(CreateAccountTableQuery, connection);
        CreateAccountTableCommand.ExecuteNonQuery();

        using var CreateDocumentTableCommand = new SQLiteCommand(CreateDocumentTableQuery, connection);
        CreateDocumentTableCommand.ExecuteNonQuery();

        using var CreateDocumentFilesTableCommand = new SQLiteCommand(CreateDocumentFilesTableQuery, connection);
        CreateDocumentFilesTableCommand.ExecuteNonQuery();

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

        using var CreateCreditCardTableCommand = new SQLiteCommand(CreateCreditCardTableQuery, connection);
        CreateCreditCardTableCommand.ExecuteNonQuery();

        using var CreateSecureNoteTableCommand = new SQLiteCommand(CreateSecureNoteTableQuery, connection);
        CreateSecureNoteTableCommand.ExecuteNonQuery();

        using var CreateCustomFieldTableCommand = new SQLiteCommand(CreateCustomFieldTableQuery, connection);
        CreateCustomFieldTableCommand.ExecuteNonQuery();

        using var CreateFolderTableCommand = new SQLiteCommand(CreateFolderTableQuery, connection);
        CreateFolderTableCommand.ExecuteNonQuery();


        connection.Close();

    }



}
