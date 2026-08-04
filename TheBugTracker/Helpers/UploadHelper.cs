using TheBugTracker.Models;
using TheBugTracker.Client.Helpers;

namespace TheBugTracker.Helpers
{
    public static class UploadHelper
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
    }
}