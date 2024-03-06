using System.Diagnostics;
using Cyber_Vault.Utils;
using System.Drawing;

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
        var ThumbsFolderPath = DatabaseHelper.ThumbsFolderPath;
        var thumbPath = Path.Combine(ThumbsFolderPath, $"{domain.ToLower()}.png");
        if (File.Exists(thumbPath))
        {        
            thumbExists = true;
        }
        return thumbExists;
    }


    // Store Thumb
    public static async void StoreThumb(string domain, string uri, ThumbType type)
    {
        if(string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(uri))
        {               
            return;
        }

        var ThumbsFolderPath = DatabaseHelper.ThumbsFolderPath;
        var ThumbPath = Path.Combine(ThumbsFolderPath, $"{domain.ToLower()}.png");

        if (File.Exists(ThumbPath))
        {
            File.Delete(ThumbPath);
        }

        if (type == ThumbType.Local)
        {
            File.Copy(uri, ThumbPath, true);
        }
        else if (type == ThumbType.Remote)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var bitmap = new Bitmap(stream);

                bitmap.Save(ThumbPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                Debug.WriteLine($"Failed to download image. Status code: {response.StatusCode}");
            }
        }
    }


    // Get Thumb Path
    public static string? GetThumbPath(string domain)
    {    
        var ThumbsFolderPath = DatabaseHelper.ThumbsFolderPath;
        var ThumbPath = Path.Combine(ThumbsFolderPath, $"{domain.ToLower()}.png");

        if (!File.Exists(ThumbPath))
        {
            return null;
        }

        return ThumbPath;
    }

}
