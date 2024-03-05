using System.Data.SQLite;
using System.Diagnostics;
using Cyber_Vault.Utils;
using Microsoft.UI.Xaml.Media.Imaging;

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
    public static void StoreThumb(string domain, string uri, ThumbType type)
    {
        byte[]? imageBytes = null;
        if(type == ThumbType.Local)
        {        
            imageBytes = GetImageBytesFromFile(uri);
        }
        else if(type == ThumbType.Remote)
        {        
            imageBytes = DownloadImage(uri).Result;
        }

        if (imageBytes == null)
        {
            return;
        }

        using var connection = new SQLiteConnection(DatabaseHelper.ThumbsDBConnectionString);
        connection.Open();

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
            var imageBytes = await webClient.GetByteArrayAsync(url);
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


    // Get Image by Domain
    public static async Task<BitmapImage?> GetImage(string domain)
    {
        var imageBytes = GetImageBytes(domain);
        try
        {
            if (imageBytes != null && imageBytes.Length > 0)
            {
                using var stream = new MemoryStream(imageBytes);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
                return bitmapImage;
            }
            else
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/DefaultThumb.png"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting bytes to BitmapImage: {ex.Message}");
        }

        return null;
    }

    // Update Thumb in DB
    public static void UpdateThumb(string domain, string uri, ThumbType type)
    {
        byte[]? imageBytes = null;
        if(type == ThumbType.Local)
        {               
            imageBytes = GetImageBytesFromFile(uri);
        }
        else if(type == ThumbType.Remote)
        {               
            imageBytes = DownloadImage(uri).Result;
        }

        if (imageBytes == null)
        {        
            return;
        }

        using var connection = new SQLiteConnection(DatabaseHelper.ThumbsDBConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection);
        command.CommandText = "UPDATE AccountThumbnails SET Image = @Image WHERE Domain = @Domain";
        command.Parameters.AddWithValue("@Domain", domain);
        command.Parameters.AddWithValue("@Image", imageBytes);

        command.ExecuteNonQuery();

        connection.Close();
    }


}
