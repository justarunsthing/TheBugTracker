using TheBugTracker.Models;

namespace TheBugTracker.Helpers
{
    public static class ImageHelper
    {
        public static readonly string DefaultProfilePictureUrl = "/img/default-profile-picture.jpg";

        public static async Task<FileUpload> GetImageUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] data = ms.ToArray();

            if (ms.Length > 1 * 1024 * 1024)
            {
                throw new Exception("The image size cannot exceed 1 MB.");
            }

            var imageUpload = new FileUpload
            {
                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType
            };

            return imageUpload;
        }
    }
}