using TheBugTracker.Models;
using TheBugTracker.Client.Helpers;
using System.Text.RegularExpressions;

namespace TheBugTracker.Helpers
{
    public static partial class UploadHelper
    {
        public static readonly string DefaultProfilePictureUrl = "/img/default-profile-picture.jpg";

        public static async Task<FileUpload> GetFileUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] data = ms.ToArray();

            if (ms.Length > BrowserFileHelper.MaxFileSize)
            {
                throw new IOException("The selected file exceeds the maximum allowed size.");
            }

            var upload = new FileUpload
            {
                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType
            };

            return upload;
        }

        public static FileUpload GetFileUpload(string dataUrl)
        {
            GroupCollection matchGroups = DataUrlRegex().Match(dataUrl).Groups;

            if (matchGroups.ContainsKey("type") && matchGroups.ContainsKey("data"))
            {
                string contentType = matchGroups["type"].Value;
                string base64String = matchGroups["data"].Value;
                byte[] imageData = Convert.FromBase64String(base64String);

                if (imageData.Length <= BrowserFileHelper.MaxFileSize)
                {
                    FileUpload fileUpload = new()
                    {
                        Id = Guid.NewGuid(),
                        Data = imageData,
                        Type = contentType
                    };

                    return fileUpload;
                }
            }
            
            throw new IOException("The data url is invalid or exceeds the maximum allowed size.");
        }

        [GeneratedRegex(@"data:(?<type>.+?);base64,(?<data>.+)")]
        private static partial Regex DataUrlRegex();
    }
}