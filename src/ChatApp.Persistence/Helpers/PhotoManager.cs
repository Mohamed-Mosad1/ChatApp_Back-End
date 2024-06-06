using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Persistence.Helpers
{
    public static class PhotoManager
    {
        // Upload Photo
        public static async Task<string> UploadPhotoAsync(this IWebHostEnvironment webHost, IFormFile file, string pathName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            string root = "wwwroot";
            string directoryPath = Path.Combine(root, "Images", pathName);
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(directoryPath, uniqueFileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            try
            {
                using (var fileStream = new FileStream(Path.Combine(webHost.ContentRootPath, filePath), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while uploading the photo {ex.Message}");
            }

            // Return the relative path to be stored in the database
            return Path.Combine("Images", pathName, uniqueFileName).Replace("\\", "/");
        }

        // Remove Photo
        public static void RemovePhoto(this IWebHostEnvironment webHost, string oldFileName)
        {
            if (!string.IsNullOrEmpty(oldFileName))
            {
                oldFileName = oldFileName.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                string root = "wwwroot";
                string oldPath = Path.Combine(webHost.ContentRootPath, root, oldFileName);
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
            }
        }
    }
}
