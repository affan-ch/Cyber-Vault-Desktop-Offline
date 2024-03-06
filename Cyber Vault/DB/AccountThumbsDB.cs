using System.Data.SQLite;
using System.Diagnostics;
using Cyber_Vault.Utils;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace Cyber_Vault.DB;

internal class AccountThumbsDB
{
    public enum ThumbType
    {    
        Local,
        Remote
    }


    // Check if Thumb Exists
    public static bool IsThumbExist(string domain)
    {
        var thumbExists = false;
        using (var connection = new SQLiteConnection(DatabaseHelper.ThumbsDBConnectionString))
        {
            connection.Open();
            domain = domain.ToLower();

            using var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT * FROM AccountThumbnails WHERE Domain = @Domain";
            command.Parameters.AddWithValue("@Domain", domain);

            using var reader = command.ExecuteReader();
            thumbExists = reader.Read();

            connection.Close();
        }
        return thumbExists;
    }


    // Store Thumb in DB
    public static async Task StoreThumbAsync(string domain, string uri, ThumbType type)
    {
        if(string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(uri))
        {               
            return;
        }
        byte[]? imageBytes = null;
        if(type == ThumbType.Local)
        {        
            imageBytes = GetImageBytesFromFile(uri);
        }
        else if(type == ThumbType.Remote)
        {        
            Debug.WriteLine("Downloading image from: " + uri);
            imageBytes = (await DownloadImage(uri).ConfigureAwait(false));
            Debug.WriteLine("Image Downloaded");
        }

        if (imageBytes == null)
        {
            Debug.WriteLine("Image bytes are null");
            return;
        }

        using var connection = new SQLiteConnection(DatabaseHelper.ThumbsDBConnectionString);
        connection.Open();
        domain = domain.ToLower();
        using var command = new SQLiteCommand(connection);
        command.CommandText = "INSERT INTO AccountThumbnails (Domain, Image) VALUES (@Domain, @Image)";
        command.Parameters.AddWithValue("@Domain", domain);
        command.Parameters.AddWithValue("@Image", imageBytes);

        command.ExecuteNonQuery();

        connection.Close();
    }


    // Get Image Bytes by Domain from DB
    public static byte[]? GetImageBytes(string domain)
    {
        byte[]? imageBytes = null;
        using (var connection = new SQLiteConnection(DatabaseHelper.ThumbsDBConnectionString))
        {   
            connection.Open();
        
            using var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT Image FROM AccountThumbnails WHERE Domain = @Domain";
            command.Parameters.AddWithValue("@Domain", domain);
        
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                imageBytes = (byte[])reader["Image"];
            }

            connection.Close();
        }
        return imageBytes;
    }


    // Get Image Bytes by API GET Request 
    public static async Task<byte[]?> DownloadImage(string url)
    {
        try
        {
            using var webClient = new HttpClient();
            var imageBytes = await webClient.GetByteArrayAsync(url).ConfigureAwait(false);
            return imageBytes;
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"Error downloading image: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

        return null;
    }


    // Get Image Bytes from Local File Location
    public static byte[]? GetImageBytesFromFile(string filePath)
    {
        try
        {
            return File.ReadAllBytes(filePath);
        }
        catch (Exception ex)
        {
        
            Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        return null;
    }


    // Update Thumb in DB
    public static async Task UpdateThumbAsync(string domain, string uri, ThumbType type)
    {
        if(string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(uri))
        {        
            return;
        }

        byte[]? imageBytes = null;
        if(type == ThumbType.Local)
        {               
            imageBytes = GetImageBytesFromFile(uri);
        }
        else if(type == ThumbType.Remote)
        {               
            imageBytes = (await DownloadImage(uri).ConfigureAwait(false));
        }

        if (imageBytes == null)
        {        
            return;
        }

        using var connection = new SQLiteConnection(DatabaseHelper.ThumbsDBConnectionString);
        connection.Open();
        domain = domain.ToLower();
        using var command = new SQLiteCommand(connection);
        command.CommandText = "UPDATE AccountThumbnails SET Image = @Image WHERE Domain = @Domain";
        command.Parameters.AddWithValue("@Domain", domain);
        command.Parameters.AddWithValue("@Image", imageBytes);

        command.ExecuteNonQuery();

        connection.Close();
    }


}
