using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using Cyber_Vault.DB;
using Cyber_Vault.Utils;
using Cyber_Vault.BL;

namespace Cyber_Vault.DL;
internal class DocumentDL
{
    private static readonly List<Document> documents = new();

    // Add Document
    public static void AddDocument(Document document)
    {
        documents.Add(document);
    }

    // Update Document
    public static void UpdateDocument(Document document)
    {
        var index = documents.FindIndex(c => c.Id == document.Id);
        documents[index] = document;
    }

    // Delete Document
    public static void DeleteDocument(int id)
    {
        var document = documents.FirstOrDefault(c => c.Id == id);
        if(document != null)
        {
            documents.Remove(document);
        }
    }

    // Get All Documents
    public static List<Document> GetDocuments()
    {
    return documents; 
    }

    // Get Document by Id
    public static Document? GetDocumentById(int id)
    {
        return documents.FirstOrDefault(c => id == c.Id);
    }

    // Clear Document List
    public static void ClearDocuments()
    {
        documents.Clear();
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

        using var command = new SQLiteCommand(connection)
        {
            CommandText = "SELECT * FROM Document"
        };

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var UsernamePtr = IntPtr.Zero;
            var PasswordPtr = IntPtr.Zero;
            try
            {
                UsernamePtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
                PasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetPasswordFromMemory()!);
                var username = Marshal.PtrToStringUni(UsernamePtr);
                var password = Marshal.PtrToStringUni(PasswordPtr);

                var document = new Document
                (
                    Id: int.Parse(reader["Id"].ToString() ?? "0"),
                    Type: EncryptionHelper.Decrypt(reader["Type"].ToString() ?? "", username + password),
                    Title: EncryptionHelper.Decrypt(reader["Title"].ToString() ?? "", username + password),
                    Document1: Encoding.UTF8.GetBytes(EncryptionHelper.Decrypt(reader["Document1"].ToString() ?? "", username + password)),
                    Document2: Encoding.UTF8.GetBytes(EncryptionHelper.Decrypt(reader["Document2"].ToString() ?? "", username + password)),
                    DateAdded: reader["DateAdded"].ToString() ?? "",
                    DateModified: reader["DateModified"].ToString() ?? ""
                );
                documents.Add(document);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(UsernamePtr);
                Marshal.ZeroFreeGlobalAllocUnicode(PasswordPtr);
            }
        }

        connection.Close();
    }


}

