using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.DB;
using Cyber_Vault.Utils;
using Cyber_Vault.BL;

namespace Cyber_Vault.DL;
internal class DocumentDL
{
    private static readonly List<Document> documents = new();
    private static readonly List<DocumentFile> documentFiles = new();

    // Add Document
    public static void AddDocument(Document document)
    {
        documents.Add(document);
    }

    // Add Document File
    public static void AddDocumentFile(DocumentFile documentFile)
    {
        documentFiles.Add(documentFile);
    }

    // Update Document
    public static void UpdateDocument(Document document)
    {
        var index = documents.FindIndex(c => c.Id == document.Id);
        documents[index] = document;
    }

    // Update Document File
    public static void UpdateDocumentFile(DocumentFile documentFile)
    {
        var index = documentFiles.FindIndex(c => c.Id == documentFile.Id);
        documentFiles[index] = documentFile;
    }

    // Delete Document
    public static void DeleteDocument(int id)
    {
        var document = documents.FirstOrDefault(c => c.Id == id);
        if (document != null)
        {
            documents.Remove(document);
        }
    }

    // Delete Document File
    public static void DeleteDocumentFile(int id)
    {
        var documentFile = documentFiles.FirstOrDefault(c => c.Id == id);
        if (documentFile != null)
        {
            documentFiles.Remove(documentFile);
        }
    }

    // Get All Documents
    public static List<Document> GetDocuments()
    {
        return documents;
    }

    // Get All Document Files
    public static List<DocumentFile> GetDocumentFiles()
    {
        return documentFiles;
    }

    // Get Document by Id
    public static Document? GetDocumentById(int id)
    {
        return documents.FirstOrDefault(c => id == c.Id);
    }

    // Get Document File by Id
    public static DocumentFile? GetDocumentFileById(int id)
    {
        return documentFiles.FirstOrDefault(c => id == c.Id);
    }

    // Clear Document List
    public static void ClearDocuments()
    {
        documents.Clear();
    }

    // Clear Document File List
    public static void ClearDocumentFiles()
    {
        documentFiles.Clear();
    }

    // Get All Documents by Document Type
    public static List<Document> GetDocumentsByType(string documentType)
    {
        return documents.Where(c => c.Type == documentType).ToList();
    }

    // Load Documents Into List from SQLite Database
    public static void LoadDocumentsFromDatabase()
    {
        using var connection = new SQLiteConnection(DatabaseHelper.ConnectionString);
        connection.Open();

        var UsernamePtr = IntPtr.Zero;
        var PasswordPtr = IntPtr.Zero;
        try
        {
            UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
            PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
            var username = Marshal.PtrToStringUni(UsernamePtr);
            var password = Marshal.PtrToStringUni(PasswordPtr);

            // Load Documents
            using var command = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM Document"
            };

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var document = new Document
                {
                    Id = int.Parse(reader["Id"].ToString() ?? "0"),
                    Type = EncryptionHelper.Decrypt(reader["Type"].ToString() ?? "", username + password),
                    Title = EncryptionHelper.Decrypt(reader["Title"].ToString() ?? "", username + password),
                    DateAdded = EncryptionHelper.Decrypt(reader["DateAdded"].ToString() ?? "", username + password),
                    DateModified = EncryptionHelper.Decrypt(reader["DateModified"].ToString() ?? "", username + password)
                };
                documents.Add(document);
            }

            // Load Document Files
            using var command2 = new SQLiteCommand(connection)
            {
                CommandText = "SELECT * FROM DocumentFile"
            };

            using var reader2 = command2.ExecuteReader();

            while (reader2.Read())
            {
                var documentFile = new DocumentFile
                {
                    Id = int.Parse(reader2["Id"].ToString() ?? "0"),
                    DocumentId = int.Parse(reader2["DocumentId"].ToString() ?? "0"),
                    FileName = EncryptionHelper.Decrypt(reader2["FileName"].ToString() ?? "", username + password),
                    FileType = EncryptionHelper.Decrypt(reader2["FileType"].ToString() ?? "", username + password),
                    FileContent = EncryptionHelper.DocumentDecrypt(reader2["FileContent"] as byte[] ?? Array.Empty<byte>(), username + password)
                };
                documentFiles.Add(documentFile);
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

